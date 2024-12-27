import React, { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import "./home.css";
import { getUserInfo, isUserLogin } from "../../../hooks/useLogin";
import { User } from "../../../model/User";
import { BlogResponse } from "../../../model/Blog";
import {
  ADD_BLOG,
  BASE_URL,
  GET_BLOGS,
  GET_NOTIFICATION_BY_USER,
  LIKE_BLOG,
  UN_LIKE_BLOG,
  UPDATE_NOTIFICATION_READ_STATUS,
} from "../../../constants/API";
import { DoCallAPIWithToken } from "../../../services/HttpService";
import { HTTP_OK } from "../../../constants/HTTPCode";
import { getWebSocketClient } from "../../../hooks/websocket";
import { getTimeAgo } from "../../../hooks/useTime";
import Comments from "../../../components/comment/comment";
import { CommentReponse } from "../../../model/Comment";
import DataLoader from "../../../components/lazyLoadComponent/DataLoader";

interface Notification {
  id: string;
  contentNotification: string;
  dateCreate: string;
  isRead: boolean;
  type: string;
  targetId: string;
}

const Home: React.FC = () => {
  const isLogin = isUserLogin();
  const navigate = useNavigate();
  const [isNotificationOpen, setIsNotificationOpen] = useState(false);
  const notificationsRef = useRef<HTMLDivElement>(null);
  const bellIconRef = useRef<HTMLDivElement>(null);
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [currentUser, setCurrentUser] = useState<User | null>(getUserInfo());
  const [content, setContent] = useState<string>("");
  const [isLoading, setIsLoading] = useState(false);
  const [comments, setComments] = useState<CommentReponse[]>([]);
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [blogs, setBlogs] = useState<BlogResponse[]>([]);
  const [isLoadingLike, setIsLoadingLike] = useState(false);
  const [commentsState, setCommentsState] = useState<{
    [key: string]: boolean;
  }>({});

  const handleCommentClick = (blogId: string) => {
    setCommentsState((prevState) => ({
      ...prevState,
      [blogId]: !prevState[blogId],
    }));
    doCallGetComment(blogId);
  };

  const handleLikeClick = (blogId: string) => {
    doCallLikeBlog(blogId);
  };

  const handleUnLikeClick = (blogId: string) => {
    doCallUnLikeBlog(blogId);
  };

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      setImageFile(file);
    }
  };
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

  const doAddBlog = () => {
    setIsLoading(false);
    const formData = new FormData();
    if (imageFile) {
      formData.append("file", imageFile);
    }
    formData.append("content", content);
    setIsLoading(true);
    DoCallAPIWithToken(ADD_BLOG, "post", formData)
      .then((res) => {
        if (res.status === HTTP_OK) {
          setContent("");
          setImageFile(null);
        }
      })
      .catch((err) => {})
      .finally(() => setIsLoading(false));
  };

  const doCallGetBlog = () => {
    setIsLoading(true);
    DoCallAPIWithToken(GET_BLOGS, "get")
      .then((res) => {
        setBlogs(res.data.result);
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  const doCallGetComment = (blogId: string) => {
    DoCallAPIWithToken(
      BASE_URL + `/comments/commentsByBlog?blogId=${blogId}`,
      "GET"
    ).then((res) => {
      if (res.status === HTTP_OK) {
        setComments(res.data.result);
      }
    });
  };

  const doCallLikeBlog = (blogId: string) => {
    console.log("du lieu neee blgidđ", blogId);
    setIsLoadingLike(true);
    const URL = LIKE_BLOG + "?blogId=" + blogId;
    DoCallAPIWithToken(URL, "POST")
      .then((res) => {
        if (res.status === HTTP_OK) {
          setBlogs((prevBlogs) =>
            prevBlogs.map((blog) =>
              blog.id === blogId ? res.data.result : blog
            )
          );
        }
      })
      .catch((err) => {})
      .finally(() => setIsLoadingLike(false));
  };

  const doCallUnLikeBlog = (blogId: string) => {
    console.log("du lieu neee blgidđ", blogId);
    setIsLoadingLike(true);
    const URL = UN_LIKE_BLOG + "?blogId=" + blogId;
    DoCallAPIWithToken(URL, "POST")
      .then((res) => {
        if (res.status === HTTP_OK) {
          setBlogs((prevBlogs) =>
            prevBlogs.map((blog) =>
              blog.id === blogId ? res.data.result : blog
            )
          );
        }
      })
      .catch((err) => {})
      .finally(() => setIsLoadingLike(false));
  };

  const updateBlogLikes = (blogsUpdate: BlogResponse) => {
    setBlogs((prevBlogs) =>
      prevBlogs.map((blog) => (blog.id === blogsUpdate.id ? blogsUpdate : blog))
    );
  };

  const hanldeAddComment = (comment: any) => {
    const commentRequest = {
      blogId: comment.blogId,
      commentContent: comment.commentContent,
      userId: comment.userId,
      replies: comment.replies,
    };
    console.log("du lieuuu neee", commentRequest);
    client.publish({
      destination: "/app/comment",
      body: JSON.stringify(commentRequest),
    });
  };

  const handldeAddReply = (replyRequest: any) => {
    console.log("du lieuuu neee", replyRequest);
    client.publish({
      destination: "/app/reply",
      body: JSON.stringify(replyRequest),
    });
  };

  const client = getWebSocketClient();
  const blogRefs = useRef<{ [key: string]: HTMLElement | null }>({});
  const [selectedBlogId, setSelectedBlogId] = useState<string | null>(null);
  useEffect(() => {
    // Kiểm tra xem phần tử có tồn tại và cuộn tới không
    if (selectedBlogId && blogRefs.current[selectedBlogId]) {
      blogRefs.current[selectedBlogId]?.scrollIntoView({
        behavior: "smooth",
        block: "start", // Cuộn tới đầu phần tử
      });
    }
  }, [selectedBlogId]);
  useEffect(() => {
    if (isLogin) {
      FetchNotificationByUser();
      doCallGetBlog();
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
      client.subscribe("/topic/notificattions", (content) => {
        const newNotification = JSON.parse(content.body);
        setNotifications((prevNotifications) => [
          ...prevNotifications,
          newNotification,
        ]);
      });

      client.subscribe("/topic/blogs", (content) => {
        const newBlog = JSON.parse(content.body);
        setBlogs((prev) => {
          const updatedBlogs = [...prev, newBlog];
          updatedBlogs.sort((a, b) => {
            const dateA = new Date(a.dateCreate).getTime();
            const dateB = new Date(b.dateCreate).getTime();

            if (isNaN(dateA) || isNaN(dateB)) {
              console.error("Invalid date:", a.dateCreate, b.dateCreate);
              return 0;
            }
            return dateB - dateA;
          });

          return updatedBlogs;
        });
      });

      client.subscribe("/topic/blog/like", (content) => {
        const updateBlog = JSON.parse(content.body);
        updateBlogLikes(updateBlog);
      });

      client.subscribe("/topic/comments", (content) => {
        const newComment = JSON.parse(content.body);
        setComments((prev) => [...prev, newComment]);
      });
      client.subscribe("/topic/replies", (content) => {
        const newReply = JSON.parse(content.body);
        setComments((prevComments) =>
          prevComments.map((comment) =>
            comment.id === newReply.commentId
              ? { ...comment, replies: [...comment.replies, newReply] }
              : comment
          )
        );
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
    setSelectedBlogId(notification.targetId);
  };

  return (
    <>
      <DataLoader isLoading={isLoading}></DataLoader>
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
                    <img className="avatar-img" src={currentUser?.imageUrl} />
                    <div className="user-info">
                      <h4>{currentUser?.fullName}</h4>
                      <p>{currentUser?.email}</p>
                    </div>
                  </div>
                  <ul className="user-menu-list">
                    <li onClick={() => navigate("/")}>Danh sách bài viết</li>
                  </ul>
                  <ul
                    className="user-menu-list"
                    onClick={() => setIsNotificationOpen(true)}
                  >
                    <li>Thông báo</li>
                  </ul>
                  <ul className="user-menu-list">
                    <li onClick={() => navigate("/userProfile")}>
                      Hồ sơ tài khoản
                    </li>
                    <li>Thông tin về easyBloggg</li>
                  </ul>
                  <ul className="user-menu-list">
                    <li onClick={handleLogout}>Đăng xuất tài khoản</li>
                  </ul>
                  <div className="user-menu-footer">
                    <span>Ngôn ngữ</span>
                    <span>Tiếng Việt &#127760;</span>
                  </div>
                </div>
              </div>
            ) : (
              <button
                className="btn btn-primary py-4 px-lg-5 d-none d-lg-block"
                onClick={handleLogin}
              >
                Tham Gia Ngay<i className="fa fa-arrow-right ms-3"></i>
              </button>
            )}
          </div>
        </div>
      </nav>

      <div className="container" style={{ marginBottom: "120px" }}>
        <div className="row">
          <div className="col-lg-8">
            <div className="card shadow-none border">
              <div className="card-body">
                <div className="form-floating mb-3">
                  <textarea
                    className="form-control"
                    placeholder="Leave a comment here"
                    id="floatingTextarea2"
                    style={{ height: "137px" }}
                    value={content}
                    onChange={(e) => setContent(e.target.value)}
                  ></textarea>
                  <label htmlFor="floatingTextarea2" className="p-7">
                    Nội dung bài viết ở đâyyyyy
                  </label>
                </div>
                <div className="d-flex align-items-center gap-2">
                  <div className="card-body text-center">
                    <img
                      className="img-account-profile rounded-circle mb-2"
                      src={imageFile ? URL.createObjectURL(imageFile) : ""}
                      alt="Profile"
                      style={{ width: "40%" }}
                    />
                    <div className="small font-italic text-muted mb-4">
                      Tệp tin JPG hoặc PNG không được lớn quá 10 MB
                    </div>
                    <input
                      className="form-control"
                      type="file"
                      onChange={handleFileChange}
                    />
                  </div>
                </div>
                <button className="btn btn-primary ms-auto" onClick={doAddBlog}>
                  Đăng bài viết
                </button>
              </div>
            </div>
            <div
              className="text-center wow fadeInUp  mt-5"
              data-wow-delay="0.1s"
            >
              <h6 className="section-title bg-white text-center text-primary px-3">
                Các Bài Viết
              </h6>
              <h1 className="mb-5">Danh sách tất cả bài viết và ảnh</h1>
            </div>
            {blogs &&
              blogs.map((blog) => (
                <div
                  className="card mt-5 "
                  key={blog.id}
                  ref={(el) => (blogRefs.current[blog.id] = el)}
                  id={blog.id}
                >
                  <div className="card-body border-bottom">
                    <div className="d-flex align-items-center gap-3">
                      <img
                        src={blog.userAvatarUrl}
                        alt=""
                        className="rounded-circle"
                        width="40"
                        height="40"
                      />
                      <h6 className="fw-semibold mb-0 fs-4">
                        {blog.userFullName}
                      </h6>
                      <span>
                        <span className="p-1 bg-light rounded-circle d-inline-block"></span>{" "}
                        {getTimeAgo(blog.dateCreate)}
                      </span>
                    </div>
                    <p className="text-dark my-3">{blog.content}</p>
                    {blog.imageUrl && (
                      <img
                        src={blog.imageUrl}
                        alt=""
                        className="img-fluid rounded-4 w-100 object-fit-cover"
                        style={{ height: "360px" }}
                      />
                    )}
                    <hr />
                    <div className="d-flex align-items-center my-3">
                      <div className="d-flex align-items-center gap-2">
                        <button
                          className="text-white d-flex align-items-center justify-content-center bg-primary p-2 fs-4 border-0 rounded-circle"
                          onClick={() =>
                            blog.statusLikeByUser
                              ? handleUnLikeClick(blog.id)
                              : handleLikeClick(blog.id)
                          }
                        >
                          {isLoadingLike && !blog.statusLikeByUser ? (
                            <i className="fas fa-spinner fa-spin"></i>
                          ) : (
                            <i
                              className={
                                blog.statusLikeByUser
                                  ? "fa fa-thumbs-up"
                                  : "far fa-thumbs-up"
                              }
                            ></i>
                          )}
                        </button>
                        <span className="text-dark fw-semibold">
                          {blog.likeCount}
                        </span>
                      </div>
                      <div className="d-flex align-items-center gap-2 ms-4">
                        <button
                          className="text-white d-flex align-items-center justify-content-center bg-secondary border-0 p-2 fs-4 rounded-circle"
                          onClick={() => handleCommentClick(blog.id)}
                        >
                          <i className="fa fa-comments"></i>
                        </button>
                        <span className="text-dark fw-semibold">
                          {blog.commentCount}
                        </span>
                      </div>
                      <button
                        className="text-dark ms-auto d-flex align-items-center justify-content-center bg-transparent p-2 fs-4 border-0 rounded-circle"
                        onClick={() => console.log("Share clicked")}
                      >
                        <i className="fa fa-share"></i>
                      </button>
                    </div>
                    {commentsState[blog.id] && comments && (
                      <div className="position-relative">
                        <div className="p-4 rounded-2 mb-3">
                          <Comments
                            blogId={blog.id}
                            comments={comments}
                            onAddComment={hanldeAddComment}
                            onAddReply={handldeAddReply}
                          />
                        </div>
                      </div>
                    )}
                  </div>
                </div>
              ))}
          </div>
        </div>
      </div>
    </>
  );
};
export default Home;
