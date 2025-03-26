import React, { useEffect, useState } from "react";
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
import "./PostDetail.css";

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

interface PostDetailProps {
  post: {
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
  };
  onClose: () => void;
}

const PostDetail: React.FC<PostDetailProps> = ({ post, onClose }) => {
  const [currentImageIndex, setCurrentImageIndex] = useState(0);

  useEffect(() => {
    // Add class when component mounts
    document.body.classList.add("modal-open");

    // Remove class when component unmounts
    return () => {
      document.body.classList.remove("modal-open");
    };
  }, []);

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
    <div className="post-detail-overlay" onClick={onClose}>
      <div
        className="post-detail-container"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="post-detail-image">
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

        <div className="post-detail-content">
          {/* Header */}
          <div className="post-detail-header">
            <div className="post-detail-author">
              <img
                src={post.author.avatar}
                alt={post.author.name}
                className="author-avatar"
              />
              <span className="author-name">{post.author.name}</span>
            </div>
            <button className="close-button" onClick={onClose}>
              ×
            </button>
          </div>

          {/* Comments Section */}
          <div className="post-detail-comments">
            {/* Original Post */}
            <div className="post-detail-original">
              <img
                src={post.author.avatar}
                alt={post.author.name}
                className="author-avatar"
              />
              <div className="comment-content">
                <span className="author-name">{post.author.name}</span>
                <p>{post.content}</p>
                <span className="comment-time">{post.createdAt}</span>
              </div>
            </div>

            {/* Comments List */}
            {post.comments.map((comment) => (
              <div key={comment.id} className="comment-item">
                <img
                  src={comment.user.avatar}
                  alt={comment.user.name}
                  className="author-avatar"
                />
                <div className="comment-content">
                  <span className="author-name">{comment.user.name}</span>
                  <p>{comment.content}</p>
                  <div className="comment-actions">
                    <span className="comment-time">{comment.createdAt}</span>
                    <span className="comment-likes">{comment.likes} likes</span>
                    <button className="reply-button">Reply</button>
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* Actions Bar */}
          <div className="post-detail-actions">
            <div className="action-buttons">
              <button
                className={`action-button ${post.isLiked ? "liked" : ""}`}
              >
                {post.isLiked ? <FaHeart /> : <FaRegHeart />}
              </button>
              <button className="action-button">
                <FaComment />
              </button>
              <button className="action-button">
                <FaShare />
              </button>
              <button
                className={`action-button ${post.isSaved ? "saved" : ""}`}
              >
                {post.isSaved ? <FaBookmark /> : <FaRegBookmark />}
              </button>
            </div>
            <div className="post-detail-info">
              <div className="likes-count">{post.likes} likes</div>
              <div className="post-time">{post.createdAt}</div>
            </div>
          </div>

          {/* Comment Input */}
          <div className="post-detail-comment-input">
            <input
              type="text"
              placeholder="Add a comment..."
              className="comment-input"
            />
            <button className="post-button" disabled={false}>
              Post
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PostDetail;
