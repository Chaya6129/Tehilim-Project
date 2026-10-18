import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// השרת מאשר CORS רק עבור http://localhost:5173, ולכן הפורט קבוע ו-strictPort פעיל:
// אם הפורט תפוס, Vite ייכשל במקום לעבור בשקט לפורט אחר.
export default defineConfig({
  plugins: [react()],
  server: {
    host: 'localhost',
    port: 5173,
    strictPort: true,
  },
  preview: {
    host: 'localhost',
    port: 5173,
    strictPort: true,
  },
});
