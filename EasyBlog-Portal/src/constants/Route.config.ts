import { ApplicationRoute } from "../model/Route";
import LazyLoadComponent from "../components/lazyLoadComponent/LazyLoadComponent";
export const RoutesConfig: ApplicationRoute[] = [
  {
    path: "/",
    component: LazyLoadComponent(import("../pages/Client/Home/Home")),
    isProtected: true,
    isAdmin: false,
  },
  
  {
    path: "/403",
    component: LazyLoadComponent(import("../pages/Errors/403/Code403")),
    isProtected: false,
    isAdmin: false,
  },
  {
    path: "/404",
    component: LazyLoadComponent(import("../pages/Errors/404/Code404")),
    isProtected: false,
    isAdmin: false,
  },
  {
    path: "/login",
    component: LazyLoadComponent(
      import("../pages/Client/Authentication/Login")
    ),
    isProtected: false,
    isAdmin: false,
  },
  {
    path: "/register",
    component: LazyLoadComponent(
      import("../pages/Client/Authentication/Register")
    ),
    isProtected: false,
    isAdmin: false,
  },

  {
    path: "/forgetPassword",
    component: LazyLoadComponent(
      import("../pages/Client/Authentication/ForgetPassword")
    ),
    isProtected: false,
    isAdmin: false,
  },


  {
    path: "/liveStream",
    component: LazyLoadComponent(import("../pages/Client/LiveStream/LiveStream")),
    isProtected: false,
    isAdmin: false,
  },

  {
    path: "/liveStreamList",
    component: LazyLoadComponent(import("../pages/Client/LiveStream/LiveStreamList")),
    isProtected: false,
    isAdmin: false,
  },
  


 

  {
    path: "/userProfile",
    component: LazyLoadComponent(import("../pages/Client/UserSetting/UserSetting")),
    isProtected: true,
    isAdmin: false,
  },
];
