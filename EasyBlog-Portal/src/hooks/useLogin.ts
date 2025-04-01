
import { User, UserProfile } from "../model/User";

const isUserLogin = (): boolean => {
  const storedAuth = localStorage.getItem("authentication");
  if (!storedAuth) return false;

  try {
    const authData: { token: string } = JSON.parse(storedAuth);
    return !!authData.token; 
  } catch (error) {
    console.error("Lỗi parse JSON:", error);
    return false;
  }
};


const getCredentials = (): string => {
  const storedAuth = localStorage.getItem("authentication");
  if (!storedAuth) return "";

  try {
    const authData = JSON.parse(storedAuth);
    return authData.token || "";

  } catch (error) {
    console.error("Lỗi parse JSON:", error);
    return "";
  }
};


const getUserInfo = () => {
  if (localStorage.getItem("user_info") != null) {
    const userInfo: UserProfile = JSON.parse(localStorage.getItem("user_info") || "");
    return userInfo;
  } else {
    return null;
  }
};

const hasAdminRole = () => {
  const storedUser = localStorage.getItem("user_info");
  if (!storedUser) return false; 

  try {
    const user: User = JSON.parse(storedUser);
    return user.role === "Admin"; 
  } catch (error) {
    console.error("Lỗi parse JSON:", error);
    return false;
  }
};

export { isUserLogin, getUserInfo, getCredentials, hasAdminRole };