import React, { ReactNode } from "react";
import footerImg from "../../../../assets/img/auth.png";
import styles from "./AuthenticationShared.module.css";

interface AuthenSharedProps {
  children: ReactNode;
  title?: string;
  subtitle?: string;
}

const AuthenticationShared: React.FC<AuthenSharedProps> = ({
  children,
  title = "Chào mừng bạn trở lại! 🎉",
  subtitle = "Đăng nhập để chia sẻ những bài viết của bạn",
}) => {
  return (
    <div className={styles.auth_container}>
      <div className={styles.auth_card}>
        <div className={styles.auth_logo}>
          <svg
            className={styles.auth_logo_svg}
            viewBox="0 0 24 24"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M19 3H5C3.89543 3 3 3.89543 3 5V19C3 20.1046 3.89543 21 5 21H19C20.1046 21 21 20.1046 21 19V5C21 3.89543 20.1046 3 19 3Z"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
            />
            <path
              d="M7 7H17"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
            />
            <path
              d="M7 12H17"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
            />
            <path
              d="M7 17H13"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
            />
          </svg>
          <span className={styles.auth_logo_text}>EASYBLOG</span>
        </div>

        <div className={styles.auth_content}>
          <h4 className={styles.auth_title}>{title}</h4>
          <p className={styles.auth_subtitle}>{subtitle}</p>
          {children}
        </div>
      </div>

      <img
        src={footerImg}
        className={styles.auth_bg_image}
        alt="background-decoration"
      />
    </div>
  );
};

export default AuthenticationShared;
