import React, { useState } from "react";
import {
  FaUser,
  FaCog,
  FaImage,
  FaVideo,
  FaLink,
  FaSmile,
} from "react-icons/fa";
import "./Home.css";
import ClientShared from "../Shared/ClientShared";
import Post from "../../../components/Post/Post";

interface Comment {
  id: number;
  user: {
    name: string;
    avatar: string;
  };
  content: string;
  createdAt: string;
  likes: number;
}

interface Post {
  id: number;
  author: {
    name: string;
    avatar: string;
  };
  images: string[];
  content: string;
  likes: number;
  comments: Comment[];
  createdAt: string;
  isLiked: boolean;
  isSaved: boolean;
}

const Home: React.FC = () => {
  const [posts, setPosts] = useState<Post[]>([
    {
      id: 1,
      author: {
        name: "John Doe",
        avatar: "https://i.pravatar.cc/150?img=1",
      },
      content: "Chuyến du lịch cuối tuần tuyệt vời! 🌊✨ #Travel #Weekend",
      images: [
        "https://source.unsplash.com/random/800x600?beach",
        "https://source.unsplash.com/random/800x600?sunset",
        "https://source.unsplash.com/random/800x600?vacation",
      ],
      likes: 120,
      comments: [
        {
          id: 1,
          user: {
            name: "Alice Smith",
            avatar: "https://i.pravatar.cc/150?img=2",
          },
          content: "Tuyệt vời quá! Nhìn phong cảnh đẹp quá 😍",
          createdAt: new Date().toISOString(),
          likes: 5,
        },
        {
          id: 2,
          user: {
            name: "Bob Johnson",
            avatar: "https://i.pravatar.cc/150?img=3",
          },
          content: "Chúc bạn có chuyến đi vui vẻ! 🌴",
          createdAt: new Date().toISOString(),
          likes: 3,
        },
      ],
      createdAt: new Date().toISOString(),
      isLiked: false,
      isSaved: false,
    },

    {
      id: 2,
      author: {
        name: "Jane Smith",
        avatar: "https://i.pravatar.cc/150?img=4",
      },
      content:
        "Những khoảnh khắc đáng nhớ trong chuyến đi Đà Lạt 🌸 #DaLat #Travel",
      images: [
        "https://source.unsplash.com/random/800x600?mountain",
        "https://source.unsplash.com/random/800x600?flowers",
        "https://source.unsplash.com/random/800x600?coffee",
        "https://source.unsplash.com/random/800x600?landscape",
      ],
      likes: 85,
      comments: [],
      createdAt: new Date().toISOString(),
      isLiked: false,
      isSaved: false,
    },

    {
      id: 3,
      author: {
        name: "Mike Wilson",
        avatar: "https://i.pravatar.cc/150?img=7",
      },
      content:
        "Bữa tiệc sinh nhật tuyệt vời với những người bạn! 🎂 #Birthday #Friends",
      images: [],
      likes: 156,
      comments: [
        {
          id: 5,
          user: {
            name: "Emma Davis",
            avatar: "https://i.pravatar.cc/150?img=8",
          },
          content: "Chúc mừng sinh nhật nha! 🎉🎈",
          createdAt: new Date().toISOString(),
          likes: 6,
        },
        {
          id: 6,
          user: {
            name: "Frank Miller",
            avatar: "https://i.pravatar.cc/150?img=9",
          },
          content: "Bánh sinh nhật nhìn ngon quá! 🎂",
          createdAt: new Date().toISOString(),
          likes: 3,
        },
      ],
      createdAt: new Date().toISOString(),
      isLiked: false,
      isSaved: false,
    },
  ]);

  const suggestedUsers = [
    {
      id: "1",
      name: "Alice Smith",
      avatar: "https://i.pravatar.cc/150?img=2",
      followers: 1234,
    },
    {
      id: "2",
      name: "Bob Johnson",
      avatar: "https://i.pravatar.cc/150?img=3",
      followers: 856,
    },
    {
      id: "3",
      name: "Carol White",
      avatar: "https://i.pravatar.cc/150?img=4",
      followers: 2341,
    },
  ];

  const handleLike = (postId: number) => {
    setPosts(
      posts.map((post) => {
        if (post.id === postId) {
          return {
            ...post,
            isLiked: !post.isLiked,
            likes: post.isLiked ? post.likes - 1 : post.likes + 1,
          };
        }
        return post;
      })
    );
  };

  const handleSave = (postId: number) => {
    setPosts(
      posts.map((post) => {
        if (post.id === postId) {
          return {
            ...post,
            isSaved: !post.isSaved,
          };
        }
        return post;
      })
    );
  };

  return (
    <ClientShared>
      <div className="home-container">
        <div className="left-sidebar">
          <div className="user-profile">
            <img
              src="https://i.pravatar.cc/150?img=1"
              alt="User avatar"
              className="user-avatar"
            />
            <div className="user-info">
              <h3 className="user-name">John Doe</h3>
              <p className="user-bio">Web Developer | Tech Enthusiast</p>
            </div>
            <button className="settings-button">
              <FaCog />
            </button>
          </div>

          <div className="suggested-users">
            <div className="section-header">
              <h3>Đề xuất cho bạn</h3>
              <button className="see-all">Xem tất cả</button>
            </div>
            <div className="suggested-list">
              {suggestedUsers.map((user) => (
                <div key={user.id} className="suggested-user">
                  <img
                    src={user.avatar}
                    alt={user.name}
                    className="suggested-avatar"
                  />
                  <div className="suggested-info">
                    <h4>{user.name}</h4>
                    <p>{user.followers} người theo dõi</p>
                  </div>
                  <button className="follow-button">Theo dõi</button>
                </div>
              ))}
            </div>
          </div>

          <div className="trending-topics">
            <div className="section-header">
              <h3>Chủ đề nổi bật</h3>
              <button className="see-all">Xem tất cả</button>
            </div>
            <div className="topics-list">
              <div className="topic-item">
                <span className="topic-tag">#Travel</span>
                <span className="topic-posts">1.2k bài viết</span>
              </div>
              <div className="topic-item">
                <span className="topic-tag">#Food</span>
                <span className="topic-posts">856 bài viết</span>
              </div>
              <div className="topic-item">
                <span className="topic-tag">#Lifestyle</span>
                <span className="topic-posts">2.3k bài viết</span>
              </div>
            </div>
          </div>
        </div>

        <div className="main-content">
          <div className="posts-container">
            {posts.map((post) => (
              <Post
                key={post.id}
                post={post}
                onLike={handleLike}
                onSave={handleSave}
              />
            ))}
          </div>
        </div>

        <div className="right-sidebar">
          <div className="create-post">
            <div className="create-post-header">
              <h3>Tạo bài viết mới</h3>
            </div>
            <div className="create-post-content">
              <textarea
                placeholder="Bạn đang nghĩ gì?"
                className="post-textarea"
              />
              <div className="post-actions-bar">
                <button className="post-action-button">
                  <FaImage />
                  <span>Ảnh</span>
                </button>
                <button className="post-action-button">
                  <FaVideo />
                  <span>Video</span>
                </button>
                <button className="post-action-button">
                  <FaLink />
                  <span>Liên kết</span>
                </button>
                <button className="post-action-button">
                  <FaSmile />
                  <span>Cảm xúc</span>
                </button>
              </div>
              <button className="publish-button">Đăng bài</button>
            </div>
          </div>
        </div>
      </div>
    </ClientShared>
  );
};

export default Home;
