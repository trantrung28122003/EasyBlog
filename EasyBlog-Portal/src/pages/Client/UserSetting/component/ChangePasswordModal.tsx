import React, { useState, useEffect } from "react";
import "./ChangePasswordModal.css";
import { useNavigate } from "react-router-dom";
import { HTTP_OK } from "../../../../constants/HTTPCode";
import { CHANGE_PASSWORD_BY_USER } from "../../../../constants/API";
import { DoCallAPIWithToken } from "../../../../services/HttpService";

interface ChangePasswordModalProps {
  isVisible: boolean;
  onClose: () => void;
}

const ChangePasswordModal: React.FC<ChangePasswordModalProps> = ({
  isVisible,
  onClose,
}) => {
  const navigate = useNavigate();
  const [oldPassword, setOldPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [countdown, setCountdown] = useState(5);
  const [isRedirecting, setIsRedirecting] = useState(false);

  const resetForm = () => {
    setOldPassword("");
    setNewPassword("");
    setConfirmPassword("");
    setErrorMessage("");
    setCountdown(5);
    setIsRedirecting(false);
  };

  const handleSubmit = async () => {
    if (!oldPassword || !newPassword || !confirmPassword) {
      setErrorMessage("Tất cả các trường mật khẩu đều là bắt buộc.");
      return;
    }
    if (newPassword.length < 6) {
      setErrorMessage("Mật khẩu mới phải có ít nhất 6 ký tự.");
      return;
    }
    if (newPassword !== confirmPassword) {
      setErrorMessage("Mật khẩu mới và xác nhận mật khẩu không khớp.");
      return;
    }
    try {
      const URL = CHANGE_PASSWORD_BY_USER;
      const changePasswordRequest = {
        oldPassword: oldPassword,
        newPassword: newPassword,
        confirmPassword: confirmPassword,
      };
      const response = await DoCallAPIWithToken(
        URL,
        "POST",
        changePasswordRequest
      );
      if (response.status === HTTP_OK) {
        setSuccessMessage("Đổi mật khẩu thành công!");
        setErrorMessage("");
        startCountdown();
      } else if (response.data.result === "Mật khẩu cũ không đúng") {
        setErrorMessage(response.data.result);
      }
    } catch (error) {
      setErrorMessage("Đã xảy ra lỗi, vui lòng thử lại.");
    }
  };

  const startCountdown = () => {
    setIsRedirecting(true);
    const interval = setInterval(() => {
      setCountdown((prev) => {
        if (prev === 1) {
          clearInterval(interval);
          navigate("/login");
          return 0;
        }
        return prev - 1;
      });
    }, 1000);
  };

  const handleOutsideClick = (e: React.MouseEvent<HTMLDivElement>) => {
    if (e.target === e.currentTarget) {
      resetForm();
      onClose();
    }
  };

  if (!isVisible) return null;

  return (
    <div className="change-password-modal-overlay" onClick={handleOutsideClick}>
      <div className="change-password-modal-container">
        <button
          className="change-password-close-button"
          onClick={() => {
            resetForm();
            onClose();
          }}
        >
          &times;
        </button>
        <h3 className="change-password-modal-header">Đổi mật khẩu</h3>
        {successMessage && (
          <div
            className="change-password-alert alert alert-success"
            role="alert"
          >
            {successMessage}
          </div>
        )}
        {errorMessage && (
          <div
            className="change-password-alert alert alert-danger"
            role="alert"
          >
            {errorMessage}
          </div>
        )}
        <form>
          <label className="change-password-form-label" htmlFor="oldPassword">
            Mật khẩu cũ
          </label>
          <input
            type="password"
            id="oldPassword"
            className="change-password-form-control form-control"
            placeholder="Nhập mật khẩu cũ"
            value={oldPassword}
            onChange={(e) => setOldPassword(e.target.value)}
          />

          <label className="change-password-form-label" htmlFor="newPassword">
            Mật khẩu mới
          </label>
          <input
            type="password"
            id="newPassword"
            className="change-password-form-control form-control"
            placeholder="Nhập mật khẩu mới"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
          />

          <label
            className="change-password-form-label"
            htmlFor="confirmPassword"
          >
            Xác nhận mật khẩu mới
          </label>
          <input
            type="password"
            id="confirmPassword"
            className="change-password-form-control form-control"
            placeholder="Nhập lại mật khẩu mới"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
          />
          <p className="text-center">
            <a
              style={{
                textDecoration: "underline",
                color: "#06BBCC",
                cursor: "pointer",
              }}
              onClick={() => navigate("/forgetPassword")}
            >
              Bạn quên mật khẩu?
            </a>
          </p>
          <button
            type="button"
            className="change-password-btn btn btn-primary"
            onClick={handleSubmit}
          >
            Đổi mật khẩu
          </button>
        </form>

        {isRedirecting && (
          <div className="change-password-redirect-info">
            <p style={{ marginTop: "10px" }}>
              Bạn sẽ được chuyển hướng đến trang đăng nhập sau{" "}
              <strong>{countdown} giây</strong>
            </p>
            <button
              className="btn-redirect btn-link"
              onClick={() => navigate("/login")}
            >
              Chuyển ngay đến trang đăng nhập
            </button>
          </div>
        )}
      </div>
    </div>
  );
};

export default ChangePasswordModal;
