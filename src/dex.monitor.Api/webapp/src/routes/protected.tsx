import { RouteObject } from "react-router-dom";
import DefaultLayout from "../layouts/DefaultLayout";
import GlobalLoader from "@components/GlobalLoader";
import MainPage from "@pages/Main";
import DexsPage from "@pages/Dexs";
import ChainsPage from "@pages/Chains";

const protectedRoutes: RouteObject[] = [
  {
    path: "/",
    element: <DefaultLayout />,
    errorElement: <GlobalLoader />,
    children: [
      { path: "", element: <MainPage /> },
      { path: "main", element: <MainPage /> },
      { path: "dexs", element: <DexsPage /> },
      { path: "chains", element: <ChainsPage /> },
    ],
  },
];

export default protectedRoutes;
