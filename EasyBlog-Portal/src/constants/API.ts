// const BASE_URL = "https://sd75skpq-8080.asse.devtunnels.ms";
const BASE_URL = "https://localhost:5000";
/* 
  authentication
*/
const BASE_URL_AUTH = BASE_URL + "/api/authentication";
const LOGIN_URL = BASE_URL_AUTH + "/login";
const LOGIN_WITH_GOOGLE = BASE_URL_AUTH + "/loginWithGoogle"
const REGISTER_URL = BASE_URL_AUTH + "/register";

/* 
  authentication
*/





/*
  user
*/

const BASE_URL_USER = BASE_URL + "/api/users";
const GET_USER_INFO_URL = BASE_URL_USER + "/profile";
const UPDATE_PROFILE_USER = BASE_URL + "/customer/updateProfile"
const CHANGE_PASSWORD_BY_USER = BASE_URL + "/customer/changePassword"
const FORGOT_PASSWORD_URL = BASE_URL + "/email/sendVerificationCode"
const VERIFY_CODE_URL = BASE_URL + "/email/verifyCode"
const RESET_PASSWORD = BASE_URL +"/customer/resetPassword"
/*
  users
*/

/**
 * home
 */


const GET_NOTIFICATION_BY_USER = BASE_URL + "/api/notifications/current-user"

const GET_TOP_AUTHORS_POST = BASE_URL + "/api/posts/top-authors-post"
const UPDATE_NOTIFICATION_READ_STATUS = BASE_URL + "/customer/updateNotificationStatusIsRead"
const ADD_BLOG = BASE_URL +"/blog/addBlog"
const LIKE_BLOG = BASE_URL +"/blog/like"
const UN_LIKE_BLOG = BASE_URL +"/blog/unLike"
const GET_BLOGS = BASE_URL +"/blog"
const STATUS_LIKE_BLOG = BASE_URL +"/customer/status"

/**
 * home
 */




/**
 * post
 */
const BASE_URL_POST = BASE_URL + "/api/posts";
const GET_ALL_POST = BASE_URL_POST + "/";
const ADD_POST = BASE_URL_POST + "/create";


/**
 * post
 */


/**
 * comment
 */
const BASE_URL_COMMENT = BASE_URL + "/api/comments";
const ADD_COMMENT = BASE_URL_COMMENT + "/create";
const UPDATE_COMMENT = BASE_URL_COMMENT + "/update";


/**
 * comment
 */

export {
  BASE_URL,
  LOGIN_URL,
  GET_USER_INFO_URL,
  LOGIN_WITH_GOOGLE,
  REGISTER_URL,
  UPDATE_PROFILE_USER,
  CHANGE_PASSWORD_BY_USER,
  FORGOT_PASSWORD_URL,
  VERIFY_CODE_URL,
  RESET_PASSWORD,
  GET_NOTIFICATION_BY_USER,
  UPDATE_NOTIFICATION_READ_STATUS ,
  ADD_BLOG,
  GET_BLOGS,
  LIKE_BLOG,
  STATUS_LIKE_BLOG,
  UN_LIKE_BLOG,
  GET_ALL_POST,
  ADD_POST,
  ADD_COMMENT,
  UPDATE_COMMENT,
  GET_TOP_AUTHORS_POST
};
