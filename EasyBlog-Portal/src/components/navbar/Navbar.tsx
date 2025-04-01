import React, { useEffect, useRef, useState } from "react";
import { getUserInfo, isUserLogin } from "../../hooks/useLogin";
import { useNavigate, Link } from "react-router-dom";
import "./Navbar.css";
import {
  GET_NOTIFICATION_BY_USER,
  UPDATE_NOTIFICATION_READ_STATUS,
} from "../../constants/API";
import { DoCallAPIWithToken } from "../../services/HttpService";
import { HTTP_OK } from "../../constants/HTTPCode";
import { getTimeAgo } from "../../hooks/useTime";
import { UserProfile } from "../../model/User";
import {
  FaSignOutAlt,
  FaUserEdit,
  FaBook,
  FaInfoCircle,
  FaGlobe,
  FaGraduationCap,
  FaCertificate,
  FaComments,
} from "react-icons/fa";
import { IoHomeOutline } from "react-icons/io5";
import { IoNotificationsOutline } from "react-icons/io5";
import { CiBookmark } from "react-icons/ci";
import { GoPeople } from "react-icons/go";
import * as signalR from "@microsoft/signalr";
import { ApplicationResponse } from "../../model/BaseResponse";
interface Notification {
  id: string;
  message: string;
  dateCreate: string;
  isRead: boolean;
  type: string;
}

const Navbar: React.FC = () => {
  const navigate = useNavigate();
  const [isNotificationOpen, setIsNotificationOpen] = useState(false);
  const notificationsRef = useRef<HTMLDivElement>(null);
  const bellIconRef = useRef<HTMLDivElement>(null);
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [currentUserProfile, setCurrentUserProfile] =
    useState<UserProfile | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const fetchNotificationByUser = async () => {
    setIsLoading(true);
    try {
      const res = await DoCallAPIWithToken(GET_NOTIFICATION_BY_USER, "GET");

      if (res.status === HTTP_OK) {
        const data: ApplicationResponse<Notification[]> = res.data;
        if (data.isSuccess) {
          setNotifications(data.results);
        } else {
          console.error("Lỗi khi lấy bài viết:", data.errors);
        }
      }
    } catch (error) {
      console.error("Lỗi khi gọi API:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const unreadNotificationsCount = notifications.filter(
    (notification) => !notification.isRead
  ).length;

  const getNotificationIcon = (type: string) => {
    switch (type) {
      case "NewPost":
        return <FaComments className="notification-type-icon" />;
      case "NewComment":
        return <FaCertificate className="notification-type-icon" />;

      default:
        return <IoNotificationsOutline className="notification-type-icon" />;
    }
  };
  const updateNotificationReadStatus = async (notificationId: string) => {
    try {
      const URL = `${UPDATE_NOTIFICATION_READ_STATUS}?notificationId=${notificationId}`;
      const response = await DoCallAPIWithToken(URL, "POST");
      if (response.status === HTTP_OK) {
      }
    } catch (error) {}
  };

  const handleMarkAllAsRead = async () => {
    notifications.map(async (notification) => {
      if (!notification.isRead) {
        updateNotificationReadStatus(notification.id);
      }
    });

    setNotifications((prevNotifications) =>
      prevNotifications.map((notification) => ({
        ...notification,
        isRead: true,
      }))
    );
  };

  const handleNotificationClick = (notification: Notification) => {
    if (!notification.isRead) {
      updateNotificationReadStatus(notification.id);
      const notificationId = notification.id;
      setNotifications((prevNotifications) =>
        prevNotifications.map((notification) =>
          notification.id === notificationId
            ? { ...notification, isRead: true }
            : notification
        )
      );
    }
  };
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null
  );
  const hubUrl = "https://localhost:5000/notificationHub";

  useEffect(() => {
    const userInfo = getUserInfo();
    if (userInfo) {
      setCurrentUserProfile(userInfo);
      fetchNotificationByUser();
    }
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl)
      .configureLogging(signalR.LogLevel.Information)
      .build();

    setConnection(connection);

    connection
      .start()
      .then(() => {
        if (currentUserProfile?.id) {
          connection
            .invoke("RegisterUser", currentUserProfile?.id)
            .then(() => {})
            .catch((err) => {
              console.error("❌ Lỗi khi đăng ký người dùng:", err);
            });
        }

        connection.on("ReceiveNotification", (notification) => {
          if (notification.userId === currentUserProfile?.id) {
            setNotifications((prev) => [...prev, notification]);
          }
        });
      })
      .catch((err) => {
        console.error("❌ Kết nối thất bại", err);
      });

    return () => {
      if (connection?.state === signalR.HubConnectionState.Connected) {
        console.log("🚪 Rời nhóm...");
        connection
          .invoke("LeavePostGroup", "d56c6ba7-16cf-4d78-b127-270b0e3a763f")
          .catch(console.error);
        connection.stop();
      }
    };
  }, [currentUserProfile?.id]);

  const handleLogout = () => {
    localStorage.clear();
    window.location.reload();
    navigate("/");
  };

  return (
    <>
      <nav className="navbar navbar-expand-lg bg-white navbar-light shadow sticky-top p-0">
        <a
          href="/"
          className="navbar-brand d-flex align-items-center px-4 px-lg-5"
        >
          <h2 className="m-0 text-primary">
            <i className="fa fa-book me-3"></i>Easy Blog
          </h2>
        </a>
        <button
          type="button"
          className="navbar-toggler me-4"
          data-bs-toggle="collapse"
          data-bs-target="#navbarCollapse"
        >
          <span className="navbar-toggler-icon"></span>
        </button>
        <div className="collapse navbar-collapse" id="navbarCollapse">
          <div className="navbar-nav ms-auto p-4 p-lg-0">
            <>
              <Link to="/" className="nav-link">
                <IoHomeOutline className="nav-icon" />
                <span>Trang chủ</span>
              </Link>
              <Link to="/explore" className="nav-link">
                <GoPeople className="nav-icon" />
                <span>Khám phá</span>
              </Link>
              <Link to="/bookmarks" className="nav-link">
                <CiBookmark className="nav-icon" />
                <span>Đã lưu</span>
              </Link>

              <div className="position-relative me-4 my-auto" ref={bellIconRef}>
                <div
                  className="nav-link notification-link"
                  onClick={() => setIsNotificationOpen(!isNotificationOpen)}
                >
                  <IoNotificationsOutline className="nav-icon" />
                  {unreadNotificationsCount > 0 && (
                    <span className="notification-badge">
                      {unreadNotificationsCount}
                    </span>
                  )}
                  <span>Thông báo</span>
                </div>
                {isNotificationOpen && (
                  <div className="notification-dropdown" ref={notificationsRef}>
                    <div className="notification-header">
                      <div style={{ fontSize: "20px" }}>Thông báo</div>
                      <button
                        className="mark-read"
                        onClick={handleMarkAllAsRead}
                      >
                        Đánh dấu tất cả đã đọc
                      </button>
                    </div>
                    <div className="notification-list">
                      {notifications.map((notification, index) => (
                        <div
                          key={index}
                          className={`notification-item ${
                            notification.isRead ? "read" : ""
                          }`}
                          onClick={() => handleNotificationClick(notification)}
                        >
                          <div className="notification-content">
                            <div className="notification-icon">
                              {getNotificationIcon(notification.type)}
                            </div>
                            <div className="notification-text">
                              <p>{notification.message}</p>
                              <span>{getTimeAgo(notification.dateCreate)}</span>
                              {!notification.isRead && (
                                <span className="new-notification-dot"></span>
                              )}
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </div>
            </>

            <div
              className="position-relative me-4 my-auto profile-dropdown"
              onMouseEnter={() => setIsNotificationOpen(false)}
            >
              <div className="avatar-text">
                {currentUserProfile?.fullName
                  ? currentUserProfile.fullName
                      .split(" ")
                      .slice(-1)[0]
                      .charAt(0)
                      .toUpperCase()
                  : "E"}
              </div>
              <div className="user-menu">
                <div className="user-menu-header">
                  <img
                    className="avatar-img"
                    src={currentUserProfile?.avatar}
                    alt={currentUserProfile?.fullName}
                  />
                  <div className="user-info">
                    <h4>{currentUserProfile?.fullName}</h4>
                    <p>{currentUserProfile?.email}</p>
                  </div>
                </div>
                <ul className="user-menu-list">
                  <li onClick={() => navigate("/ ")}>
                    <FaBook className="menu-icon" />
                    Danh sách bài viết
                  </li>
                </ul>
                <ul
                  className="user-menu-list"
                  onClick={() => setIsNotificationOpen(true)}
                >
                  <li>
                    <IoNotificationsOutline className="menu-icon" />
                    Thông báo
                  </li>
                </ul>
                <ul className="user-menu-list">
                  <li onClick={() => navigate("/userProfile")}>
                    <FaUserEdit className="menu-icon" />
                    Hồ sơ tài khoản
                  </li>
                  <li>
                    <FaInfoCircle className="menu-icon" />
                    Thông tin về easyBloggg
                  </li>
                </ul>
                <ul className="user-menu-list">
                  <li onClick={handleLogout}>
                    <FaSignOutAlt className="menu-icon" />
                    Đăng xuất tài khoản
                  </li>
                </ul>
                <div className="user-menu-footer">
                  <span>Ngôn ngữ</span>
                  <span>
                    <FaGlobe />
                    Tiếng Việt
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </nav>
    </>
  );
};
export default Navbar;
