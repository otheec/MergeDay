import {Navigate, Outlet, useLocation} from "react-router";
import {useAuthContext} from "../context/AuthContext.tsx";
import {APP_ROUTES} from "../config/AppRoutes.ts";

export const AuthenticatedRoute = () => {
  const { isAuthenticated } = useAuthContext();

  const location = useLocation();

  if (!isAuthenticated) {
    //redirect to this route after login
    return(
      <Navigate
        to={APP_ROUTES.login}
        replace
        state={{redirectTo: location}}
      />);
  }

  return (
    <Outlet/>
  );
};
