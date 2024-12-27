import React from "react";
import { isUserLogin } from "../../../hooks/useLogin";

const AuthenticationNav: React.FC = () => {
  const isLogin = isUserLogin();
  return isLogin ? <div>UserName</div> : <button>Login</button>;
};

export default AuthenticationNav;
