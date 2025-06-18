import { RouteObject } from "react-router-dom";
import GlobalLoader from "@components/GlobalLoader";
import DefaultLayout from "@layouts/DefaultLayout";

const publicRoutes: RouteObject[] = [
  {
    path: "",
    element: <DefaultLayout />,
    errorElement: <GlobalLoader message="Ooops..." size="30%" />,
    children: [],
  },
];

export default publicRoutes;
