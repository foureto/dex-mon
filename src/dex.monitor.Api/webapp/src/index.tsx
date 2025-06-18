import { createBrowserRouter, RouterProvider } from "react-router-dom";
import { createRoot } from "react-dom/client";
import publicRoutes from "./routes/public";
import protectedRoutes from "./routes/protected";
import "./index.scss";

const container = document.getElementById("app");
// eslint-disable-next-line @typescript-eslint/no-non-null-assertion
const root = createRoot(container!);

const router = createBrowserRouter([
  ...publicRoutes,
  ...protectedRoutes,
]);

root.render(<RouterProvider router={router} />);
