import React, { useState } from "react";
import AuthenticationShared from "./Shared/AuthenticationShared";
import { useNavigate } from "react-router-dom";
import * as yup from "yup";
import {
  APIRegisterRequest,
  RegisterRequest,
} from "../../../model/Authentication";
import { Field, Form, Formik } from "formik";
import { DoCallAPIWithOutToken } from "../../../services/HttpService";
import { REGISTER_URL } from "../../../constants/API";
import { HTTP_OK } from "../../../constants/HTTPCode";
import DataLoader from "../../../components/lazyLoadComponent/DataLoader";
import styles from "./Shared/AuthenticationShared.module.css";
import { Link } from "react-router-dom";

const Register: React.FC = () => {
  const navigate = useNavigate();
  const [registerError, setRegisterError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [registerSuccess, setRegisterSuccess] = useState(false);
  const handleLogin = () => {
    localStorage.clear();
    navigate("/login");
  };

  const schema = yup.object().shape({
    email: yup
      .string()
      .email("Địa chỉ email phải hợp lệ")
      .required("Địa chỉ email là bắt buộc"),
    fullName: yup
      .string()
      .required("Họ và tên là bắt buộc")
      .max(50, "Họ và tên không được vượt quá 50 ký tự"),
    password: yup
      .string()
      .min(6, "Mật khẩu phải có ít nhất 6 ký tự")
      .required("Mật khẩu là bắt buộc"),
    confirmPassword: yup
      .string()
      .oneOf([yup.ref("password"), ""], "Mật khẩu không khớp")
      .required("Xác nhận mật khẩu là bắt buộc"),
    termAndConditions: yup
      .boolean()
      .oneOf([true], "Bạn phải chấp nhận các điều khoản và điều kiện"),
  });

  const doRegister = (account: APIRegisterRequest) => {
    const formData = new FormData();
    if (account.file) {
      formData.append("file", account.file);
    }

    formData.append("email", account.email);
    formData.append("fullName", account.fullName);
    formData.append("password", account.password);

    setIsLoading(true);
    DoCallAPIWithOutToken<APIRegisterRequest>(REGISTER_URL, "POST", formData)
      .then((res) => {
        if (res.status === HTTP_OK) {
          setRegisterSuccess(true);
        }
      })
      .catch((err) => {
        if (err.response && err.response.status === 400) {
          setRegisterError("Email đã được sử dụng.");
        } else {
          setRegisterError("Đăng ký thất bại. Vui lòng thử lại sau.");
        }
      })
      .finally(() => setIsLoading(false));
  };

  return (
    <>
      {registerSuccess && (
        <div className={styles.modal_overlay}>
          <div className={styles.success_modal}>
            <div className={styles.success_icon}>✅</div>
            <h3 className={styles.success_title}>Đăng ký thành công!</h3>
            <p className={styles.success_message}>
              Chúc mừng bạn đã đăng ký tài khoản thành công. Hãy đăng nhập để
              bắt đầu trải nghiệm!
            </p>
            <button
              onClick={handleLogin}
              className={`${styles.btn} ${styles.btn_primary}`}
            >
              Đăng nhập ngay
            </button>
          </div>
        </div>
      )}
      <AuthenticationShared
        title="Tạo tài khoản mới ✨"
        subtitle="Đăng ký để bắt đầu chia sẻ những bài viết của bạn"
      >
        <DataLoader isLoading={isLoading} />

        <Formik
          initialValues={{
            email: "",
            fullName: "",
            imageName: "",
            password: "",
            confirmPassword: "",
            file: null,
            termAndConditions: false,
          }}
          validationSchema={schema}
          onSubmit={(values: RegisterRequest) => {
            if (!values.termAndConditions) {
              setRegisterError(
                "Bạn phải chấp nhận các điều khoản và điều kiện"
              );
              return;
            }
            const requestPayload: APIRegisterRequest = {
              email: values.email,
              fullName: values.fullName,
              password: values.password,
              file: values.file,
            };
            setRegisterError(null);
            doRegister(requestPayload);
          }}
          validateOnChange
        >
          {({ touched, errors, setFieldValue, handleChange }) => (
            <Form className={styles.auth_form}>
              {(registerError ||
                (errors.termAndConditions && touched.termAndConditions)) && (
                <div className={styles.form_error_message}>
                  {registerError || errors.termAndConditions}
                </div>
              )}
              <div className={styles.form_group}>
                <Field
                  type="text"
                  id="fullName"
                  name="fullName"
                  className={styles.form_control}
                  placeholder="Nhập họ và tên"
                  autoComplete="name"
                />
                <label htmlFor="fullName" className={styles.form_label}>
                  Họ và tên
                </label>
                {errors.fullName && touched.fullName && (
                  <div className={styles.error_message}>{errors.fullName}</div>
                )}
              </div>

              <div className={styles.form_group}>
                <Field
                  type="email"
                  id="email"
                  name="email"
                  className={styles.form_control}
                  placeholder="Nhập email"
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
                  placeholder="Nhập mật khẩu"
                  autoComplete="new-password"
                />
                <label htmlFor="password" className={styles.form_label}>
                  Mật khẩu
                </label>
                {errors.password && touched.password && (
                  <div className={styles.error_message}>{errors.password}</div>
                )}
              </div>

              <div className={styles.form_group}>
                <Field
                  type="password"
                  id="confirmPassword"
                  name="confirmPassword"
                  className={styles.form_control}
                  placeholder="Nhập lại mật khẩu"
                  autoComplete="new-password"
                />
                <label htmlFor="confirmPassword" className={styles.form_label}>
                  Xác nhận mật khẩu
                </label>
                {errors.confirmPassword && touched.confirmPassword && (
                  <div className={styles.error_message}>
                    {errors.confirmPassword}
                  </div>
                )}
              </div>

              <div className={styles.form_group}>
                <input
                  type="file"
                  id="imageName"
                  className={styles.form_control}
                  onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                    if (event.currentTarget.files) {
                      setFieldValue(
                        "imageName",
                        event.currentTarget.files[0].name
                      );
                      setFieldValue("file", event.currentTarget.files[0]);
                    }
                  }}
                />
                <label htmlFor="imageName" className={styles.form_label}>
                  Ảnh đại diện
                </label>
                {errors.imageName && touched.imageName && (
                  <div className={styles.error_message}>{errors.imageName}</div>
                )}
              </div>

              <div className={styles.form_group}>
                <label className={styles.checkbox_label}>
                  <input
                    type="checkbox"
                    id="termAndConditions"
                    name="termAndConditions"
                    onChange={handleChange}
                  />{" "}
                  Tôi đồng ý với{" "}
                  <a href="#" className={styles.auth_link}>
                    Chính sách bảo mật & Các điều khoản
                  </a>
                </label>
              </div>

              <div className={styles.form_group}>
                <button
                  type="submit"
                  className={`${styles.btn} ${styles.btn_primary}`}
                >
                  Đăng ký
                </button>
              </div>

              <div
                className={styles.form_group}
                style={{ textAlign: "center" }}
              >
                <p>
                  Đã có tài khoản?{" "}
                  <Link
                    to="/login"
                    className={styles.auth_link}
                    onClick={handleLogin}
                  >
                    Đăng nhập
                  </Link>
                </p>
              </div>
            </Form>
          )}
        </Formik>
      </AuthenticationShared>
    </>
  );
};

export default Register;
