import { Route, Routes } from 'react-router-dom';
import { RequireAdmin } from './auth/RequireAdmin';
import { RequireAuth } from './auth/RequireAuth';
import { Layout } from './components/Layout';
import { AdminCategoriesPage } from './pages/AdminCategoriesPage';
import { BookPage } from './pages/BookPage';
import { CreateBookPage } from './pages/CreateBookPage';
import { EditBookPage } from './pages/EditBookPage';
import { HomePage } from './pages/HomePage';
import { LoginPage } from './pages/LoginPage';
import { MyBooksPage } from './pages/MyBooksPage';
import { NotFoundPage } from './pages/NotFoundPage';
import { RegisterPage } from './pages/RegisterPage';

export function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route index element={<HomePage />} />
        <Route path="login" element={<LoginPage />} />
        <Route path="register" element={<RegisterPage />} />
        <Route path="books/:id" element={<BookPage />} />

        <Route element={<RequireAuth />}>
          <Route path="books/new" element={<CreateBookPage />} />
          <Route path="books/:id/edit" element={<EditBookPage />} />
          <Route path="my-books" element={<MyBooksPage />} />
        </Route>

        <Route element={<RequireAdmin />}>
          <Route path="admin/categories" element={<AdminCategoriesPage />} />
        </Route>

        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
}
