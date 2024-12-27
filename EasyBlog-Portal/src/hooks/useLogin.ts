import { LoginResponse } from "../model/Authentication";
import { User } from "../model/User";

const isUserLogin = (): boolean => {
  if (localStorage.getItem("authentication") != null) {
    const authenticationInfo: LoginResponse = JSON.parse(
      localStorage.getItem("authentication") || ""
    );
    return authenticationInfo.authenticated || false;
  } else {
    return false;
  }
};

const getCredentials = (): string => {
  if (localStorage.getItem("authentication") != null) {
    const authenticationInfo: LoginResponse = JSON.parse(
      localStorage.getItem("authentication") || ""
    );
    return authenticationInfo.token || "";
  } else {
    return "";
  }
};

const getUserInfo = () => {
  if (localStorage.getItem("user_info") != null) {
    const userInfo: User = JSON.parse(localStorage.getItem("user_info") || "");
    return userInfo;
  } else {
    return null;
  }
};

const hasAdminRole = () => {
  if (localStorage.getItem("user_info") != null) {
    const res: User = JSON.parse(localStorage.getItem("user_info") || "");
    return res.roles.some((role) => role.name === "ADMIN");
  } else {
    return false;
  }
};

export { isUserLogin, getUserInfo, getCredentials, hasAdminRole };