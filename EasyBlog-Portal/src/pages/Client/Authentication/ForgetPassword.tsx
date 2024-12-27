import React, { useState, useEffect } from "react";
import { Form, Field, Formik } from "formik";
import * as yup from "yup";
import { DoCallAPIWithOutToken } from "../../../services/HttpService";
import {
  FORGOT_PASSWORD_URL,
  RESET_PASSWORD,
  VERIFY_CODE_URL,
} from "../../../constants/API";
import { HTTP_OK } from "../../../constants/HTTPCode";
import AuthenticationShared from "./Shared/AuthenticationShared";
import "./ForgetPassword.css";
import { useNavigate } from "react-router-dom";

interface VerifyCode {
  email: string;
  verificationCode: string;
}

interface ResetPassword {
  newPassword: string;
  confirmPassword: string;
}
const ForgotPassword: React.FC = () => {
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [isCodeSent, setIsCodeSent] = useState<boolean>(false);
  const [isCodeVerified, setIsCodeVerified] = useState<boolean>(false);
  const [timer, setTimer] = useState<number>(0);
  const [intervalId, setIntervalId] = useState<NodeJS.Timeout | null>(null);
  const [userEmail, setUserEmail] = useState<string>("");
  const [countdown, setCountdown] = useState(5);
  const [isRedirecting, setIsRedirecting] = useState(false);
  const sendCodeSchema = yup.object().shape({
    email: yup
      .string()
      .email("Email không hợp lệ")
      .required("Email không được để trống"),

    verificationCode: yup
      .string()
      .length(6, "Mã xác thực không hợp lệ")
      .required("Mã xác thực không được để trống"),
  });

  const resetPasswordSchema = yup.object().shape({
    newPassword: yup
      .string()
      .min(6, "Mật khẩu phải có ít nhất 6 ký tự")
      .required("Mật khẩu không được để trống"),
    confirmPassword: yup
      .string()
      .oneOf([yup.ref("newPassword")], "Mật khẩu xác nhận không khớp")
      .required("Mật khẩu xác nhận không được để trống"),
  });

  const handleForgotPassword = (email: string) => {
    setIsLoading(true);
    DoCallAPIWithOutToken(FORGOT_PASSWORD_URL, "POST", { email })
      .then((res) => {
        if (res.data.code === HTTP_OK) {
          setSuccessMessage("Mã xác nhận đã được gửi vào email của bạn");
          setErrorMessage(null);
          setIsCodeSent(true);
          setTimer(110);
        } else if (res.data.code === 404) {
          setErrorMessage(res.data.message);
        } else {
          setErrorMessage("Có lỗi xảy ra, vui lòng thử lại.");
        }
      })
      .catch(() => {
        setErrorMessage("Không thể gửi mã xác nhận. Vui lòng thử lại sau.");
      })
      .finally(() => setIsLoading(false));
  };

  const handleVerifyCode = (values: VerifyCode) => {
    setIsLoading(true);
    DoCallAPIWithOutToken(VERIFY_CODE_URL, "POST", values)
      .then((res) => {
        if (res.data.code === HTTP_OK) {
          setErrorMessage(null);
          setSuccessMessage(null);
          setIsCodeVerified(true);
          setUserEmail(values.email);
        } else {
          setErrorMessage(res.data.message);
        }
      })
      .catch(() => {
        setErrorMessage("Lỗi xác minh mã xác nhận. Vui lòng thử lại.");
      })
      .finally(() => setIsLoading(false));
  };

  const handleResetPassword = (values: ResetPassword) => {
    const payload = {
      ...values,
      email: userEmail,
    };
    setIsLoading(true);
    DoCallAPIWithOutToken(RESET_PASSWORD, "POST", payload)
      .then((res) => {
        if (res.data.code === HTTP_OK) {
          setSuccessMessage("Mật khẩu đã được thay đổi thành công.");
          setErrorMessage(null);
          startCountdown();
        } else {
          setErrorMessage(
            res.data.message || "Có lỗi xảy ra, vui lòng thử lại."
          );
        }
      })
      .catch(() => {
        setErrorMessage("Không thể thay đổi mật khẩu. Vui lòng thử lại.");
      })
      .finally(() => setIsLoading(false));
  };

  const startTimer = () => {
    if (timer > 0 && !intervalId) {
      const newIntervalId = setInterval(() => {
        setTimer((prev) => {
          if (prev <= 1) {
            clearInterval(newIntervalId);
            setIntervalId(null);
            setIsCodeSent(false);

            return 0;
          }
          return prev - 1;
        });
      }, 1000);
      setIntervalId(newIntervalId);
    }
  };

  const startCountdown = () => {
    setIsRedirecting(true);
    const interval = setInterval(() => {
      setCountdown((prev) => {
        if (prev === 1) {
          clearInterval(interval);
          navigate("/login", { replace: true });
          return 0;
        }
        return prev - 1;
      });
    }, 1000);
  };

  useEffect(() => {
    if (isCodeSent && timer > 0) {
      startTimer();
    }
  }, [isCodeSent, timer]);

  return (
    <AuthenticationShared>
      {!isCodeVerified ? (
        <div>
          <p className="mb-4" style={{ color: "#4b4b4b", textAlign: "center" }}>
            {" "}
            Nhập email của bạn lúc đăng kí toàn khoản và chúng tôi sẽ gửi mã{" "}
            khôi phục mật khẩu
          </p>
          <Formik
            initialValues={{ email: "", verificationCode: "" }}
            validationSchema={sendCodeSchema}
            onSubmit={(values: VerifyCode) => {
              console.log("Submitting values:", values);
              handleVerifyCode(values);
            }}
            validateOnChange
          >
            {({
              touched,
              errors,
              values,

              handleSubmit,
            }) => (
              <Form onSubmit={handleSubmit}>
                <div className="form-floating form-floating-outline mb-3">
                  <Field
                    name="email"
                    className="form-control"
                    type="email"
                    id="email"
                    placeholder="Nhập email"
                  />
                  <label htmlFor="email">Email</label>
                  {errors.email && touched.email && (
                    <div className="text-danger no-caret">{errors.email}</div>
                  )}
                </div>
                <div className="form-floating form-floating-outline mb-3 d-flex align-items-center">
                  <Field
                    type="text"
                    className="form-control me-2"
                    id="verificationCode"
                    name="verificationCode"
                    placeholder="Nhập mã"
                    value={
                      !isCodeSent || timer === 0 ? "" : values.verificationCode
                    }
                    disabled={!isCodeSent || timer === 0}
                    required
                  />
                  <label htmlFor="verificationCode">Mã xác thực</label>
                  <button
                    type="button"
                    className="btn btn-primary btn-send-verification"
                    disabled={isCodeSent}
                    onClick={() => handleForgotPassword(values.email)}
                  >
                    {isLoading ? (
                      <i className="fas fa-spinner fa-spin"></i>
                    ) : isCodeSent ? (
                      `Gửi lại mã ${timer}`
                    ) : (
                      "Gửi mã "
                    )}
                  </button>
                </div>
                {errors.verificationCode && touched.verificationCode && (
                  <div className="text-danger">{errors.verificationCode}</div>
                )}
                {errorMessage && (
                  <div className="text-danger text-center mb-3 no-caret">
                    {errorMessage}
                  </div>
                )}
                {successMessage && (
                  <div className="text-success text-center mb-3 mt-4 no-caret">
                    {successMessage}
                  </div>
                )}
                <button
                  type="submit"
                  className="btn btn-primary d-grid w-100 btn-verification"
                  disabled={
                    !values.verificationCode ||
                    values.verificationCode.length !== 6
                  }
                >
                  Xác minh mã
                </button>
              </Form>
            )}
          </Formik>
        </div>
      ) : (
        <Formik
          initialValues={{ newPassword: "", confirmPassword: "" }}
          validationSchema={resetPasswordSchema}
          onSubmit={(values: ResetPassword) => handleResetPassword(values)}
        >
          {({ errors, touched, handleSubmit }) => (
            <Form onSubmit={handleSubmit}>
              <div className="form-floating form-floating-outline mb-3">
                <Field
                  name="newPassword"
                  className="form-control"
                  type="password"
                  placeholder="Mật khẩu mới"
                />
                <label>Mật khẩu mới</label>
                {errors.newPassword && touched.newPassword && (
                  <div className="text-danger">{errors.newPassword}</div>
                )}
              </div>
              <div className="form-floating form-floating-outline mb-3">
                <Field
                  name="confirmPassword"
                  className="form-control"
                  type="password"
                  placeholder="Xác nhận mật khẩu"
                />
                <label>Xác nhận mật khẩu</label>
                {errors.confirmPassword && touched.confirmPassword && (
                  <div className="text-danger">{errors.confirmPassword}</div>
                )}
              </div>
              {errorMessage && (
                <div className="text-danger text-center mb-3 no-caret">
                  {errorMessage}
                </div>
              )}
              {successMessage && (
                <>
                  <div className="text-success text-center mb-3 mt-4 no-caret">
                    {successMessage}
                  </div>
                </>
              )}
              {isRedirecting ? (
                <div>
                  <p style={{ textAlign: "center" }}>
                    Bạn sẽ được chuyển đến trang đăng nhập sau{" "}
                    <strong>{countdown} giây</strong>
                  </p>
                  <button
                    className="btn-redirect btn-link"
                    onClick={() => navigate("/login")}
                  >
                    Chuyển ngay đến trang đăng nhập
                  </button>
                </div>
              ) : (
                <button
                  type="submit"
                  className="btn btn-primary d-grid w-100 btn-verification btn-reset-password"
                >
                  Đổi mật khẩu
                </button>
              )}
            </Form>
          )}
        </Formik>
      )}
    </AuthenticationShared>
  );
};

export default ForgotPassword;
