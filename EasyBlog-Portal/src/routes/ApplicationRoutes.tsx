import { Route, Routes } from "react-router-dom";
import ProtectedRoute from "./ProtectedRoute";
import { RoutesConfig } from "../constants/Route.config";
import { ApplicationRoute } from "../model/Route";
import { GoogleOAuthProvider } from "@react-oauth/google";
import { WebSocketProvider } from "../contexts/WebSocketContext";

const ApplicationRoutes = () => {
  return (
    <>
      <GoogleOAuthProvider clientId="409126728225-mn4h754gh3l2a1vhd3jjk74ug5b0dmug.apps.googleusercontent.com">
        <WebSocketProvider>
          <Routes>
            {RoutesConfig.map((route: ApplicationRoute, index: number) => {
              return route.isProtected ? (
                <Route
                  path={route.path}
                  element={
                    <ProtectedRoute isAdmin={route.isAdmin}>
                      {route.component}
                    </ProtectedRoute>
                  }
                  key={index}
                />
              ) : (
                <Route
                  path={route.path}
                  element={route.component}
                  key={index}
                />
              );
              // return (
              //   <Route path={route.path} element={route.component} key={index} />
              // );
            })}
          </Routes>
        </WebSocketProvider>
      </GoogleOAuthProvider>
    </>
  );
};

export default ApplicationRoutes;
