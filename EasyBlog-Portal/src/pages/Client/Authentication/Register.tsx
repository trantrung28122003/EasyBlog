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

const Register: React.FC = () => {
  const navigate = useNavigate();
  const [registerError, setRegisterError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const handleLogin = () => {
    localStorage.clear();
    navigate("/login");
  };

  const schema = yup.object().shape({
    userName: yup
      .string()
      .min(8, "Tên tài khoản phải có ít nhất 8 ký tự")
      .max(24, "Tên tài khoản không được vượt quá 24 ký tự")
      .required("Tên tài khoản là bắt buộc"),
    email: yup
      .string()
      .email("Địa chỉ email phải hợp lệ")
      .required("Địa chỉ email là bắt buộc"),
    fullName: yup
      .string()
      .required("Họ và tên là bắt buộc")
      .max(50, "Họ và tên không được vượt quá 50 ký tự"),
    dayOfBirth: yup.date().required("Ngày sinh là bắt buộc"),
    imageName: yup.string(),
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

    formData.append("userName", account.userName);
    formData.append("email", account.email);
    formData.append("fullName", account.fullName);
    formData.append("password", account.password);

    if (account.dayOfBirth) {
      const formattedDate = new Date(account.dayOfBirth)
        .toISOString()
        .split("T")[0];
      formData.append("dayOfBirth", formattedDate);
    }
    setIsLoading(true);
    DoCallAPIWithOutToken<APIRegisterRequest>(REGISTER_URL, "post", formData)
      .then((res) => {
        if (res.status === HTTP_OK) {
          navigate("/login");
        }
      })
      .catch((err) => {
        if (err.response && err.response.status === 400) {
          setRegisterError("Tài khoản hoặc email đã tồn tại.");
        } else {
          setRegisterError("Đăng ký thất bại. Vui lòng thử lại sau.");
        }
      })
      .finally(() => setIsLoading(false));
  };

  return (
    <AuthenticationShared>
      <DataLoader isLoading={isLoading} />
      <Formik
        initialValues={{
          userName: "",
          email: "",
          fullName: "",
          dayOfBirth: new Date("2003-12-28").toISOString(),
          imageName: "",
          password: "",
          confirmPassword: "",
          file: null,
          termAndConditions: false,
        }}
        validationSchema={schema}
        onSubmit={(values: RegisterRequest) => {
          const requestPayload: APIRegisterRequest = {
            userName: values.userName,
            email: values.email,
            fullName: values.fullName,
            dayOfBirth: values.dayOfBirth,
            password: values.password,
            file: values.file,
          };

          doRegister(requestPayload);
        }}
        validateOnChange
      >
        {({ touched, errors, setFieldValue, handleChange }) => (
          <div className="container">
            <Form>
              <div className="form-floating form-floating-outline mb-3">
                <Field
                  type="text"
                  className="form-control"
                  id="userName"
                  name="userName"
                  placeholder="Nhập UserName"
                />
                <label style={{ color: "#06BBCC" }} htmlFor="userName">
                  Tên tài khoản
                </label>
                {errors.userName && touched.userName && (
                  <div className="text-danger">{errors.userName}</div>
                )}
              </div>

              <div className="form-floating form-floating-outline mb-3">
                <Field
                  type="text"
                  className="form-control"
                  id="fullName"
                  name="fullName"
                  placeholder="Nhập Họ Và Tên"
                />
                <label style={{ color: "#06BBCC" }} htmlFor="fullName">
                  Họ Và Tên
                </label>
                {errors.fullName && touched.fullName && (
                  <div className="text-danger">{errors.fullName}</div>
                )}
              </div>

              <div className="form-floating form-floating-outline mb-3">
                <Field
                  type="text"
                  className="form-control"
                  id="email"
                  name="email"
                  placeholder="Nhập Email"
                />
                <label style={{ color: "#06BBCC" }} htmlFor="email">
                  Email
                </label>
                {errors.email && touched.email && (
                  <div className="text-danger">{errors.email}</div>
                )}
              </div>

              <div className="form-floating form-floating-outline mb-3">
                <Field
                  type="date"
                  className="form-control"
                  id="dayOfBirth"
                  name="dayOfBirth"
                  placeholder="Nhập Ngày Sinh"
                />
                <label style={{ color: "#06BBCC" }} htmlFor="dayOfBirth">
                  Ngày Sinh
                </label>
                {errors.dayOfBirth && touched.dayOfBirth && (
                  <div className="text-danger">{errors.dayOfBirth}</div>
                )}
              </div>

              <div className="form-floating form-floating-outline mb-3">
                <Field
                  type="password"
                  id="password"
                  className="form-control"
                  name="password"
                  placeholder="&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;"
                  aria-describedby="password"
                />
                <label style={{ color: "#06BBCC" }} htmlFor="password">
                  Mật Khẩu
                </label>
                {errors.password && touched.password && (
                  <div className="text-danger">{errors.password}</div>
                )}
              </div>

              <div className="form-floating form-floating-outline mb-3">
                <Field
                  type="password"
                  id="confirmPassword"
                  className="form-control"
                  name="confirmPassword"
                  placeholder="&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;"
                  aria-describedby="confirmPassword"
                />
                <label style={{ color: "#06BBCC" }} htmlFor="confirmPassword">
                  Xác Nhận Mật Khẩu
                </label>
                {errors.confirmPassword && touched.confirmPassword && (
                  <div className="text-danger">{errors.confirmPassword}</div>
                )}
              </div>

              <div className="form-floating form-floating-outline mb-3">
                <input
                  className="form-control"
                  type="file"
                  id="imageName"
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
                <label style={{ color: "#06BBCC" }} htmlFor="imageName">
                  Ảnh đại diện
                </label>
                {errors.imageName && touched.imageName && (
                  <div className="text-danger">{errors.imageName}</div>
                )}
              </div>

              <div className="mb-3">
                <input
                  type="checkbox"
                  id="termAndConditions"
                  name="termAndConditions"
                  onChange={handleChange}
                />
                <label className="form-check-label" htmlFor="termAndConditions">
                  &nbsp;Tôi đồng ý với{" "}
                  <a style={{ color: "#06BBCC" }}>
                    Chính sách bảo mật & Các điều khoản
                  </a>
                </label>
                {errors.termAndConditions && touched.termAndConditions && (
                  <div className="text-danger">{errors.termAndConditions}</div>
                )}
              </div>
              {registerError && (
                <div className="text-danger text-center mb-3">
                  {registerError}
                </div>
              )}
              <button
                style={{
                  backgroundColor: "#06BBCC",
                  borderColor: "#06BBCC",
                }}
                type="submit"
                className="btn btn-primary d-grid w-100"
              >
                Đăng Ký
              </button>
            </Form>
          </div>
        )}
      </Formik>
      <p className="text-center" style={{ marginTop: "8px" }}>
        <span>Đã Có Tài Khoản ? </span>
        <a onClick={handleLogin}>
          <span style={{ color: "#06BBCC", cursor: "pointer" }}>
            {" "}
            Đăng Nhập
          </span>
        </a>
      </p>
    </AuthenticationShared>
  );
};

export default Register;
