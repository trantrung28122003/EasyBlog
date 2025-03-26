import React, { useState } from "react";
import {
  FaHeart,
  FaRegHeart,
  FaComment,
  FaShare,
  FaBookmark,
  FaRegBookmark,
  FaChevronLeft,
  FaChevronRight,
} from "react-icons/fa";
import { format } from "date-fns";
import PostDetail from "../PostDetail/PostDetail";
import "./Post.css";

interface PostProps {
  post: {
    id: number;
    author: {
      name: string;
      avatar: string;
    };
    images: string[];
    content: string;
    likes: number;
    comments: Array<{
      id: number;
      user: {
        name: string;
        avatar: string;
      };
      content: string;
      createdAt: string;
      likes: number;
    }>;
    createdAt: string;
    isLiked: boolean;
    isSaved: boolean;
  };
  onLike?: (id: number) => void;
  onSave?: (id: number) => void;
}

// Dữ liệu mẫu cho testing

const Post: React.FC<PostProps> = ({ post, onLike, onSave }) => {
  const [showDetail, setShowDetail] = useState(false);
  const [currentImageIndex, setCurrentImageIndex] = useState(0);

  // Thêm comments mẫu vào post
  const postWithComments = {
    ...post,
    comments: post.comments?.length > 0 ? post.comments : [],
  };

  const handleCommentClick = () => {
    setShowDetail(true);
  };

  const handlePrevImage = (e: React.MouseEvent) => {
    e.stopPropagation();
    setCurrentImageIndex((prev) => (prev > 0 ? prev - 1 : prev));
  };

  const handleNextImage = (e: React.MouseEvent) => {
    e.stopPropagation();
    setCurrentImageIndex((prev) =>
      prev < post.images.length - 1 ? prev + 1 : prev
    );
  };

  return (
    <>
      <div className="post-card">
        <div className="post-header">
          <div className="post-author">
            <img
              src={post.author.avatar}
              alt={post.author.name}
              className="author-avatar"
            />
            <span className="author-name">{post.author.name}</span>
          </div>
          <button className="more-options">•••</button>
        </div>

        <div className="post-image">
          <img
            src={post.images[currentImageIndex]}
            alt={`Post by ${post.author.name}`}
          />

          {/* Navigation Arrows */}
          {post.images.length > 1 && (
            <>
              {currentImageIndex > 0 && (
                <button
                  className="image-nav-button prev"
                  onClick={handlePrevImage}
                >
                  <FaChevronLeft />
                </button>
              )}
              {currentImageIndex < post.images.length - 1 && (
                <button
                  className="image-nav-button next"
                  onClick={handleNextImage}
                >
                  <FaChevronRight />
                </button>
              )}

              {/* Image Indicators */}
              <div className="image-indicators">
                {post.images.map((_, index) => (
                  <div
                    key={index}
                    className={`indicator ${
                      index === currentImageIndex ? "active" : ""
                    }`}
                    onClick={(e) => {
                      e.stopPropagation();
                      setCurrentImageIndex(index);
                    }}
                  />
                ))}
              </div>
            </>
          )}
        </div>

        <div className="post-actions">
          <div className="left-actions">
            <button
              className={`action-button ${post.isLiked ? "liked" : ""}`}
              onClick={() => onLike?.(post.id)}
            >
              {post.isLiked ? <FaHeart /> : <FaRegHeart />}
            </button>
            <button className="action-button" onClick={handleCommentClick}>
              <FaComment />
            </button>
            <button className="action-button">
              <FaShare />
            </button>
          </div>
          <button
            className={`action-button ${post.isSaved ? "saved" : ""}`}
            onClick={() => onSave?.(post.id)}
          >
            {post.isSaved ? <FaBookmark /> : <FaRegBookmark />}
          </button>
        </div>

        <div className="post-info">
          <div className="likes-count">{post.likes} lượt thích</div>
          <div className="post-content">
            <span className="author-name">{post.author.name}</span>
            {post.content}
          </div>
          <div className="comments-count" onClick={handleCommentClick}>
            Xem tất cả {postWithComments.comments.length} bình luận
          </div>
          <div className="post-time">
            {format(new Date(post.createdAt), "dd/MM/yyyy")}
          </div>
        </div>

        <div className="post-comment">
          <input
            type="text"
            placeholder="Thêm bình luận..."
            className="comment-input"
          />
          <button className="post-button" disabled={true}>
            Đăng
          </button>
        </div>
      </div>

      {showDetail && (
        <PostDetail
          post={postWithComments}
          onClose={() => setShowDetail(false)}
        />
      )}
    </>
  );
};

export default Post;
