import React, { useEffect, useRef, useState } from "react";
import { getUserInfo, hasAdminRole, isUserLogin } from "../../hooks/useLogin";
import { useNavigate } from "react-router-dom";
import "./Navbar.css";
import {
  GET_NOTIFICATION_BY_USER,
  UPDATE_NOTIFICATION_READ_STATUS,
} from "../../constants/API";
import { DoCallAPIWithToken } from "../../services/HttpService";
import { HTTP_OK } from "../../constants/HTTPCode";
import { getTimeAgo } from "../../hooks/useTime";
import { getWebSocketClient } from "../../hooks/websocket";
import { User } from "../../model/User";
import {
  FaBell,
  FaSignOutAlt,
  FaUserEdit,
  FaBook,
  FaInfoCircle,
  FaGlobe,
} from "react-icons/fa";
import { IoNotificationsOutline } from "react-icons/io5";
import { BiLogIn, BiUserPlus } from "react-icons/bi";

interface Notification {
  id: string;
  contentNotification: string;
  dateCreate: string;
  isRead: boolean;
  type: string;
  targetId: string;
}

const Navbar: React.FC = () => {
  const isAdmin = hasAdminRole();
  const isLogin = isUserLogin();
  const navigate = useNavigate();
  const [isNotificationOpen, setIsNotificationOpen] = useState(false);
  const notificationsRef = useRef<HTMLDivElement>(null);
  const bellIconRef = useRef<HTMLDivElement>(null);
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [cartItemsCount, setCartItemsCount] = useState(0);
  const [currentUser, setCurrentUser] = useState<User | null>(getUserInfo());

  const FetchNotificationByUser = async () => {
    try {
      const URL = GET_NOTIFICATION_BY_USER;
      const response = await DoCallAPIWithToken(URL, "GET");
      if (response.status === HTTP_OK) {
        setNotifications(response.data.result);
      }
    } catch (error) {}
  };

  const handleLogout = () => {
    localStorage.clear();
    window.location.reload();
    navigate("/");
  };
  const handleLogin = () => {
    localStorage.clear();
    navigate("/login");
  };

  const client = getWebSocketClient();
  useEffect(() => {
    if (isLogin) {
      FetchNotificationByUser();
    }
    setCurrentUser(getUserInfo());
    const handleClickOutside = (event: MouseEvent) => {
      if (
        notificationsRef.current &&
        !notificationsRef.current.contains(event.target as Node) &&
        !bellIconRef.current?.contains(event.target as Node)
      ) {
        setIsNotificationOpen(false);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);

    client.onConnect = () => {
      client.subscribe("/topic/notifications", (content) => {
        const newNotification = JSON.parse(content.body);
        setNotifications((prevNotifications) => [
          ...prevNotifications,
          newNotification,
        ]);
      });
      if (currentUser?.id) {
        client.subscribe(`/user/${currentUser.id}/notifications`, (content) => {
          const newNotification = JSON.parse(content.body);
          setNotifications((prevNotifications) => [
            ...prevNotifications,
            newNotification,
          ]);
        });
      }

      if (currentUser?.id) {
        client.subscribe(`/user/${currentUser.id}/shoppingCarts`, (content) => {
          const shoppingCartItem = JSON.parse(content.body);
          console.log("WebSocket Message Received:", shoppingCartItem);
          setCartItemsCount((prevCount) => prevCount + 1);
        });
      }
    };
    client.activate();

    return () => {
      if (client.active) client.deactivate();
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [client]);

  const unreadNotificationsCount = notifications.filter(
    (notification) => !notification.isRead
  ).length;

  const getNotificationIcon = (type: string) => {
    switch (type) {
      case "COMMENT":
        return <i className="fa fa-comments"></i>;
      case "CERTIFICATE":
        return <i className="fa fa-certificate"></i>;
      case "COURSE_PURCHASE":
        return <i className="fa fa-graduation-cap"></i>;
      default:
        return <i className="fa fa-bell"></i>;
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

    if (notification.targetId != null) {
      const urlParts = notification.targetId.split("?");
      const urlLearning = urlParts[0];
      const paramsString = urlParts[1];
      const params = new URLSearchParams(paramsString);
      const trainingPartId = params.get("trainingPartId");
      navigate(`${urlLearning}`);
      sessionStorage.setItem("trainingPartId", `${trainingPartId}`);
    }
  };

  return (
    <>
      <nav className="navbar navbar-expand-lg bg-white navbar-light shadow sticky-top p-0">
        <a
          href="/"
          className="navbar-brand d-flex align-items-center px-4 px-lg-5"
        >
          <h2 className="m-0 text-primary">
            <i className="fa fa-book me-3"></i>easyyyy BLogggggg
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
            {isLogin && (
              <>
                <h5 className="position-relative me-4 my-auto">
                  Thông báo ở đây nè!
                </h5>
                <div
                  className="position-relative me-4 my-auto"
                  ref={bellIconRef}
                >
                  <i
                    className="fa fa-bell fa-2x"
                    onClick={() => setIsNotificationOpen(!isNotificationOpen)}
                    style={{ cursor: "pointer", fontSize: "30px" }}
                  >
                    {unreadNotificationsCount > 0 && (
                      <span className="notification-badge">
                        {unreadNotificationsCount}
                      </span>
                    )}
                  </i>
                  {isNotificationOpen && (
                    <div
                      className="notification-dropdown"
                      ref={notificationsRef}
                    >
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
                            onClick={() =>
                              handleNotificationClick(notification)
                            }
                          >
                            <div className="notification-content">
                              <div className="notification-icon">
                                {getNotificationIcon(notification.type)}
                              </div>
                              <div className="notification-text">
                                <p>{notification.contentNotification}</p>
                                <span>
                                  {getTimeAgo(notification.dateCreate)}
                                </span>
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
            )}
            {isLogin ? (
              <div
                className="position-relative me-4 my-auto profile-dropdown"
                onMouseEnter={() => setIsNotificationOpen(false)}
              >
                <div className="avatar-text">
                  {currentUser?.fullName
                    ? currentUser.fullName
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
                      src={currentUser?.imageUrl}
                      alt={currentUser?.fullName}
                    />
                    <div className="user-info">
                      <h4>{currentUser?.fullName}</h4>
                      <p>{currentUser?.email}</p>
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
            ) : (
              <div className="auth-buttons">
                <button
                  className="btn btn-outline-primary"
                  onClick={() => navigate("/login")}
                >
                  <BiLogIn className="btn-icon" />
                  Đăng nhập
                </button>
                <button
                  className="btn btn-primary"
                  onClick={() => navigate("/register")}
                >
                  <BiUserPlus className="btn-icon" />
                  Đăng ký
                </button>
              </div>
            )}
          </div>
        </div>
      </nav>
    </>
  );
};
export default Navbar;
