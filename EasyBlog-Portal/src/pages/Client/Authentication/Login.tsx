import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import AuthenticationShared from "./Shared/AuthenticationShared";
import styles from "./Shared/AuthenticationShared.module.css";
import { useGoogleLogin } from "@react-oauth/google";
import {
  DoCallAPIWithOutToken,
  DoCallAPIWithToken,
} from "../../../services/HttpService";
import {
  GET_USER_INFO_URL,
  LOGIN_URL,
  LOGIN_WITH_GOOGLE,
} from "../../../constants/API";
import { HTTP_OK } from "../../../constants/HTTPCode";
import { LoginRequest } from "../../../model/Authentication";
import * as yup from "yup";
import { Field, Form, Formik } from "formik";
import DataLoader from "../../../components/lazyLoadComponent/DataLoader";

const Login: React.FC = () => {
  const navigate = useNavigate();
  const [loginError, setLoginError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const schema = yup.object().shape({
    email: yup
      .string()
      .email("Email không hợp lệ")
      .required("Email không được để trống"),
    password: yup.string().required("Mật khẩu không được để trống"),
  });

  const doLogin = (user: LoginRequest) => {
    console.log("Login attempt with:", user);
    setIsLoading(true);
    DoCallAPIWithOutToken<LoginRequest>(LOGIN_URL, "post", user)
      .then((res) => {
        if (res.status === HTTP_OK && res.data.results) {
          const authData = { token: res.data.results };
          localStorage.setItem("authentication", JSON.stringify(authData));
          fetchCurrentUser();
        } else {
          console.log("Login failed with status:", res.status);
          setLoginError("Tài khoản hoặc mật khẩu không đúng.");
        }
      })
      .catch((err) => {
        console.error("Login error:", err);
        if (
          err.response &&
          (err.response.status === 404 || err.response.status === 500)
        ) {
          console.log("Login error status:", err.response.status);
          setLoginError("Tài khoản hoặc mật khẩu không đúng.");
        } else {
          setLoginError("Đăng nhập thất bại. Vui lòng thử lại sau.");
        }
      });
  };

  const googleLogin = useGoogleLogin({
    onSuccess: async (tokenResponse) => {
      try {
        const res = await DoCallAPIWithOutToken(LOGIN_WITH_GOOGLE, "POST", {
          access_token: tokenResponse.access_token,
        });
        if (res.status === 200) {
          localStorage.setItem(
            "authentication",
            JSON.stringify(res.data.results)
          );
          console.log("Authentication stored:", res.data.results);
          fetchCurrentUser();
        } else {
          setLoginError("Đăng nhập không thành công");
        }
      } catch (error) {
        console.error("Error during Google login:", error);
        setLoginError("Có lỗi xảy ra, vui lòng thử lại.");
      }
    },
  });

  const fetchCurrentUser = () => {
    console.log("Fetching current user...");
    setIsLoading(true);
    DoCallAPIWithToken(GET_USER_INFO_URL, "GET")
      .then((res) => {
        console.log("User info response:", res);
        if (res.status === HTTP_OK) {
          localStorage.setItem("user_info", JSON.stringify(res.data.results));
          navigate("/", { replace: true });
        } else {
          console.log("Failed to fetch user info:", res.status);
          navigate("/login-fail");
        }
      })
      .catch((err) => {
        console.error("Error fetching user info:", err);
        navigate("/login-fail");
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  const handleRegister = () => {
    localStorage.clear();
    navigate("/register");
  };

  const handleForgetPassword = () => {
    localStorage.clear();
    navigate("/forgetPassword");
  };

  return (
    <AuthenticationShared>
      <DataLoader isLoading={isLoading} />
      <Formik
        initialValues={{ email: "", password: "" }}
        validationSchema={schema}
        onSubmit={(values: LoginRequest) => {
          console.log("Form submitted with values:", values);
          setLoginError(null);
          doLogin(values);
        }}
        validateOnChange
      >
        {({ touched, errors, handleSubmit }) => (
          <Form className={styles.auth_form} onSubmit={handleSubmit}>
            {loginError && (
              <div className={styles.form_error_message}>{loginError}</div>
            )}
            <div className={styles.form_group}>
              <Field
                type="email"
                id="email"
                name="email"
                className={styles.form_control}
                placeholder="Nhập email của bạn"
                autoComplete="email"
              />
              <label htmlFor="email" className={styles.form_label}>
                Email
              </label>
              {errors.email && touched.email && (
                <div className={styles.error_message}>{errors.email}</div>
              )}
            </div>

            <div className={styles.form_group}>
              <Field
                type="password"
                id="password"
                name="password"
                className={styles.form_control}
                placeholder="Nhập mật khẩu của bạn"
                autoComplete="current-password"
              />
              <label htmlFor="password" className={styles.form_label}>
                Mật khẩu
              </label>
              {errors.password && touched.password && (
                <div className={styles.error_message}>{errors.password}</div>
              )}
            </div>

            {loginError && (
              <div
                className={styles.error_message}
                style={{ textAlign: "center", marginBottom: "1rem" }}
              >
                {loginError}
              </div>
            )}

            <div className={styles.form_group}>
              <button
                type="submit"
                className={`${styles.btn} ${styles.btn_primary}`}
              >
                Đăng nhập
              </button>
            </div>

            <div className={styles.form_group}>
              <Link
                to="/forget-password"
                className={styles.auth_link}
                onClick={handleForgetPassword}
              >
                Bạn quên mật khẩu?
              </Link>
            </div>

            <div className={styles.divider}>
              <span>hoặc</span>
            </div>

            <div className={styles.social_buttons}>
              <button
                type="button"
                className={`${styles.btn_social} ${styles.btn_google}`}
                onClick={() => googleLogin()}
              >
                <i className={`fab fa-google ${styles.social_icon}`}></i>
                Google
              </button>
              <button
                type="button"
                className={`${styles.btn_social} ${styles.btn_facebook}`}
                onClick={() => {
                  /* Facebook login handler */
                }}
              >
                <i className={`fab fa-facebook-f ${styles.social_icon}`}></i>
                Facebook
              </button>
            </div>

            <div className={styles.form_group} style={{ textAlign: "center" }}>
              <p>
                Chưa có tài khoản?{" "}
                <Link
                  to="/register"
                  className={styles.auth_link}
                  onClick={handleRegister}
                >
                  Đăng ký ngay
                </Link>
              </p>
            </div>
          </Form>
        )}
      </Formik>
    </AuthenticationShared>
  );
};

export default Login;
