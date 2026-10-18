# Tehilim API: מערכת לחלוקת פרקי תהילים

שרת ASP.NET Core Web API לחלוקת ספרי תהילים בין משתתפים. משתמש פותח "ספר" עם יעד (למשל "לרפואת כל חולי ישראל"), ולספר נוצרים 150 פרקים. משתתפים, מחוברים או אורחים, נוטלים פרקים לאמירה, וכל פרק ניתן ליטול פעם אחת בלבד. כשכל הפרקים נלקחו, הספר עובר אוטומטית למצב "הושלם".

> **לקוח React:** [קישור ל-repository של הלקוח](<הדבק כאן את הקישור>)

---

## המשאב המוגבל והתחרות עליו

**המשאב המוגבל הוא פרק בספר.** לכל פרק יש בעלים אחד לכל היותר, ושני משתתפים עלולים לנסות ליטול את אותו פרק כמעט באותו רגע. התחרות היא אפוא **על נטילת הפרק**.

**המנגנון: optimistic concurrency עם עמודת `Version`.**

1. לכל פרק יש עמודה `Version` מסוג `Guid`, שמוגדרת ב-EF Core כ-concurrency token.
2. בכל שמירה של פרק ששונה, `AppDbContext.SaveChangesAsync` מציב בו `Version` חדש.
3. EF Core שולח את פקודת ה-UPDATE עם תנאי: "רק אם ה-`Version` עדיין זהה לערך שנקרא". אם בינתיים משתתף אחר שמר את הפרק, ה-`Version` כבר השתנה, אף שורה לא מתעדכנת, ו-EF זורק `DbUpdateConcurrencyException`.
4. ה-Repository ממיר את החריגה ל-`ConcurrencyConflictException`, וה-Service מחזיר `Conflict`.
5. **המשתתף שמאחר מקבל 409** עם ההודעה: "הפרק נלקח זה עתה על ידי משתתף אחר, אנא בחר פרק אחר".

**סגירת הספר:** אחרי נטילה מוצלחת, עדכון מותנה (`CompleteIfNoAvailableChaptersAsync`) מעביר את הספר ל"הושלם" רק אם לא נשארו בו פרקים פנויים. הבדיקה נעשית בבסיס הנתונים ולא בזיכרון, ולכן היא נכונה גם כששני משתתפים נוטלים במקביל את שני הפרקים האחרונים. העדכון רץ עם `CancellationToken.None`, כדי שביטול הבקשה לא ישאיר ספר מלא במצב "פתוח".

---

## דרישות מוקדמות

| רכיב | גרסה |
|---|---|
| .NET SDK | 10 |
| PostgreSQL | 16 ומעלה (פותח מול PostgreSQL בקונטיינר Docker) |
| Docker | נדרש רק אם מריצים את PostgreSQL בקונטיינר |
| EF Core CLI | `dotnet tool install --global dotnet-ef` (נדרש רק ליצירת migrations חדשות) |

---

## הרצה

### 1. הפעלת PostgreSQL

אם PostgreSQL כבר מותקן מקומית, אפשר לדלג לשלב 2. אחרת, מריצים קונטיינר:

```bash
docker run --name tehilim-db -d -p 5432:5432 \
  -e POSTGRES_USER=tehilim_user \
  -e POSTGRES_PASSWORD=<סיסמה לבחירתך> \
  -e POSTGRES_DB=tehilim \
  postgres:16
```

למשתמש שב-connection string צריכה להיות הרשאה ליצור בסיס נתונים, כי השרת יוצר אותו בהרצה הראשונה, והבדיקות יוצרות בסיס נתונים זמני משלהן.

### 2. הגדרת User Secrets

הסודות לא נשמרים ב-repo. מגדירים שני מפתחות, משורש ה-solution:

| מפתח | תוכן |
|---|---|
| `ConnectionStrings:Default` | connection string ל-PostgreSQL |
| `Jwt:Key` | מפתח לחתימת ה-JWT, באורך 32 תווים לפחות |

```bash
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=tehilim;Username=<משתמש>;Password=<סיסמה>" --project Tehilim.API
dotnet user-secrets set "Jwt:Key" "<מפתח אקראי>" --project Tehilim.API
```

ליצירת מפתח אקראי, ב-PowerShell 7:

```powershell
[Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(48))
```

פרויקט הבדיקות משתמש באותם User Secrets (אותו `UserSecretsId`), ולכן אין צורך להגדיר לו אותם שוב.

### 3. הרצת השרת

```bash
dotnet run --project Tehilim.API
```

**בסביבת Development, השרת מריץ את כל ה-migrations אוטומטית בעלייה,** כולל נתוני הדמו. בסיס נתונים ריק, או כזה שעוד לא קיים, מגיע למצב מלא בלי שום פעולה ידנית.

- Swagger: `https://localhost:7245/swagger`
- API: `https://localhost:7245/api`

אם הדפדפן מתריע על תעודת HTTPS, מריצים פעם אחת:

```bash
dotnet dev-certs https --trust
```

### 4. הרצת הבדיקות

```bash
dotnet test
```

PostgreSQL חייב לרוץ, כי בדיקת ההתנגשות רצה מול בסיס נתונים אמיתי. היא יוצרת בסיס נתונים זמני בשם ייחודי, ומוחקת אותו בסיום.

### 5. הרצת הלקוח

ראה את ה-README ב-repository של הלקוח. השרת מאשר CORS עבור `http://localhost:5173`, כתובת ברירת המחדל של Vite.

---

## משתמשי דמו

ה-seed יוצר את הנתונים הבאים:

| אימייל | סיסמה | Role |
|---|---|---|
| `admin@tehilim.com` | `123456` | Admin |
| `user@tehilim.com` | `123456` | User |

בנוסף נוצרים 4 קטגוריות (רפואה, הצלחה, עילוי נשמה, ישועה), וספר פומבי פתוח אחד עם 150 פרקים, משויך לקטגוריה "רפואה". הבעלים של הספר הוא `user@tehilim.com`.

---

## ארכיטקטורה

```
Tehilim.Core     ישויות, ממשקים, DTOs, Result. ללא שום תלות חיצונית
   ↑
Tehilim.Data     EF Core, AppDbContext, Fluent API, Repositories, migrations
Tehilim.service  לוגיקה עסקית, AutoMapper, JWT, hash לסיסמאות
   ↑
Tehilim.API      Controllers, middleware, DI, NLog, Swagger
Tehilim.tests    xUnit + Moq, ובדיקת התנגשות מול PostgreSQL
```

**כיוון התלויות:** Data ← Core; Service ← Core; API ← Service, Core, Data. פרויקט Service לא מכיר את EF Core, ופרויקט Core לא מכיר אף ספרייה חיצונית.

**עקרונות:**

- **Controllers דקים:** מקבלים בקשה, מעבירים ל-Service, וממירים את התוצאה לסטטוס HTTP. אין בהם לוגיקה עסקית ואין בהם DbContext.
- **תבנית Result:** ה-Services מחזירים `Result` או `Result<T>` עם סוג כישלון (`NotFound`, `Unauthorized`, `Forbidden`, `Conflict`, `Validation`). המיפוי לסטטוס HTTP נעשה במקום אחד, ב-`ResultExtensions.ToActionResult()`.
- **DTOs בלבד:** ישויות לא נכנסות ל-API ולא יוצאות ממנו.
- **אסינכרוניות מלאה:** `async` ו-`CancellationToken` מה-Controller ועד ה-DbContext.
- **UTC בלבד:** כל התאריכים נשמרים ומוחזרים ב-UTC. ההמרה לשעון מקומי נעשית בלקוח.
- **DI:** DbContext, Repositories ו-Services רשומים כ-scoped.

---

## API

| Method | נתיב | הרשאה | תיאור |
|---|---|---|---|
| POST | `/api/auth/register` | פתוח | הרשמה. מחזיר token |
| POST | `/api/auth/login` | פתוח | התחברות. מחזיר token |
| GET | `/api/books` | פתוח | ספרים פתוחים ופומביים: חיפוש, סינון לפי קטגוריה, מיון ודפדוף |
| GET | `/api/books/mine` | מחובר | הספרים של המשתמש, כולל ספרים שאינם פומביים |
| GET | `/api/books/{id}` | פתוח | ספר בודד, כולל ספר שאינו פומבי (שיתוף בקישור ישיר) |
| POST | `/api/books` | מחובר | יצירת ספר עם 150 פרקים פנויים |
| PUT | `/api/books/{id}` | הבעלים | עדכון יעד, פומביות וקטגוריות |
| DELETE | `/api/books/{id}` | הבעלים או מנהל | מחיקת ספר |
| GET | `/api/books/{bookId}/chapters` | פתוח | 150 הפרקים של ספר |
| PATCH | `/api/books/{bookId}/chapters/{number}` | פתוח | נטילת פרק, כמשתמש מחובר או כאורח |
| GET | `/api/categories` | פתוח | רשימת הקטגוריות |
| POST | `/api/categories` | מנהל | יצירת קטגוריה |
| PUT | `/api/categories/{id}` | מנהל | שינוי שם קטגוריה |
| DELETE | `/api/categories/{id}` | מנהל | מחיקת קטגוריה (הספרים עצמם לא נמחקים) |

התיעוד המלא, עם המבנה של כל בקשה ותגובה וכל הסטטוסים האפשריים, נמצא ב-Swagger. כדי לשלוח בקשות מוגנות מתוך Swagger, מתחברים, מעתיקים את ה-`token` מהתגובה, ומדביקים אותו בכפתור **Authorize**.

**סטטוסים בשימוש:** 200, 201 (עם כותרת `Location`), 204, 400, 401, 403, 404, 409, 500.

### פורמט שגיאה אחיד

כל השגיאות של 400, 404, 409 ו-500 מוחזרות באותו מבנה:

```json
{
  "status": 409,
  "message": "הפרק נלקח זה עתה על ידי משתתף אחר, אנא בחר פרק אחר",
  "correlationId": "8f3a2c1e-4b5d-4e6f-9a7b-1c2d3e4f5a6b",
  "errors": null
}
```

בשגיאות ולידציה, `errors` מכיל פירוט לפי שדות. 401 ו-403 של מנגנון ההרשאות מוחזרים בלי גוף.

---

## אבטחה

- **JWT:** ה-token כולל את ה-claims של מזהה, שם, אימייל ו-role. בכל בקשה נבדקים ה-Issuer, ה-Audience, תוקף ה-token והחתימה, בלי מרווח זמן נוסף אחרי התפוגה.
- **סיסמאות:** נשמרות כ-hash בלבד, עם `PasswordHasher` של ASP.NET Identity.
- **הרשמה:** יוצרת תמיד משתמש עם role של `User`. משתמש `Admin` נוצר רק דרך ה-seed.
- **הרשאות:** נאכפות בשרת. בדיקת הבעלות על ספר נעשית ב-Service, והרשאות המנהל נאכפות עם `[Authorize(Roles = "Admin")]`.
- **אימייל:** מנורמל לאותיות קטנות בהרשמה ובהתחברות, כדי שהאינדקס הייחודי ב-PostgreSQL, שרגיש לאותיות גדולות וקטנות, לא יאפשר כפילויות.
- **סודות:** connection string ומפתח ה-JWT נשמרים ב-User Secrets בלבד.
- **לוג:** לא נכתבים ללוג סיסמאות, tokens או גוף של בקשות.

---

## לוגים ומעקב

- **NLog** מחובר כספק הלוגים. כל השכבות כותבות דרך `ILogger` בלבד, ושכבות Core, Data ו-Service לא מכירות את NLog.
- **יעדים:** הקונסול, וקובץ יומי בתיקייה `Tehilim.API/bin/Debug/net10.0/logs/`.
- **תבנית שורה:** זמן UTC | רמה | CorrelationId | מקור | הודעה.
- **CorrelationId:** כל בקשה מקבלת מזהה, או משתמשת במזהה שהגיע בכותרת `X-Correlation-Id`. המזהה מוחזר בכותרת התגובה ובגוף של כל שגיאה, ומופיע בכל שורות הלוג של הבקשה.
- **רמות:** Information לכל בקשה (method, נתיב, סטטוס ומשך), Warning לכל התנגשות על פרק, ו-Error לחריגות שלא טופלו.

**סדר ה-pipeline:**

```
ExceptionHandling → CorrelationId → Swagger → RequestLogging → HttpsRedirection → CORS → Authentication → Authorization → Controllers
```

---

## בסיס הנתונים

- **Code-First** עם שתי migrations: `Init` ו-`AddCategories`.
- **קשרים:** User–Book (one-to-many), Book–Chapter (one-to-many, עם מחיקה מדורגת), User–Chapter (אופציונלי), ו-Book–Category (many-to-many דרך טבלת `BookCategories`).
- **אינדקסים:** ייחודי על אימייל המשתמש, ייחודי על הצירוף של מזהה ספר ומספר פרק, ייחודי על שם קטגוריה, ואינדקס על הסטטוס והפומביות של ספר.
- **ביצועים:** `AsNoTracking` בכל קריאה. סינון, מיון ו-`Skip`/`Take` נעשים בתוך השאילתה. רשימת הספרים נבנית כ-projection עם הקטגוריות באותה שאילתה, בלי N+1. ספר בודד נטען עם `AsSplitQuery`, כדי למנוע הכפלת שורות בין הפרקים לקטגוריות.
- **Seed:** כל הערכים קבועים, כולל מזהים, hash הסיסמה, תאריכים ו-`Version`, כך שה-seed לא משתנה בין migrations. ה-sequences של עמודות ה-identity מתחילים אחרי טווח המזהים של ה-seed, כדי שרשומות חדשות לא יתנגשו בו.

---

## בדיקות

| מחלקה | מה נבדק |
|---|---|
| `ChapterServiceTests` | נטילה מוצלחת; פרק תפוס (Conflict, בלי שמירה); חריגת התנגשות בשמירה (Conflict, בלי סגירת ספר); פרק שאינו קיים (NotFound); הפעלת בדיקת סגירת הספר אחרי נטילה |
| `BookServiceTests` | יצירה של בדיוק 150 פרקים פנויים; Forbidden בעדכון בידי מי שאינו הבעלים; מחיקה בידי מנהל שאינו הבעלים |
| `ChapterConcurrencyTests` | שני `AppDbContext` נפרדים קוראים את אותו פרק; הראשון שומר; השמירה של השני זורקת `DbUpdateConcurrencyException`, והנתון של הראשון נשמר |

בדיקות היחידה משתמשות ב-Moq. בדיקת ההתנגשות רצה מול PostgreSQL אמיתי, ולא מול ספק InMemory, כי רק בסיס נתונים אמיתי מוכיח שה-concurrency token נאכף.

---

## פתרון תקלות

| תקלה | פתרון |
|---|---|
| השרת לא עולה, עם שגיאה על `JwtSettings` | `Jwt:Key` חסר ב-User Secrets, או קצר מ-32 תווים |
| השרת לא עולה, עם שגיאת connection string | `ConnectionStrings:Default` חסר ב-User Secrets |
| `permission denied to create database` | למשתמש ב-connection string אין הרשאת `CREATEDB` |
| הבדיקות נכשלות על connection string חסר | ה-`UserSecretsId` ב-`Tehilim.tests.csproj` חייב להיות זהה לזה שב-`Tehilim.API.csproj` |
| הלקוח מקבל שגיאת CORS | הלקוח חייב לרוץ על `http://localhost:5173` |
| הדפדפן חוסם את כתובת השרת | `dotnet dev-certs https --trust`, ופתיחה חד-פעמית של `https://localhost:7245/swagger` כדי לאשר את התעודה |
