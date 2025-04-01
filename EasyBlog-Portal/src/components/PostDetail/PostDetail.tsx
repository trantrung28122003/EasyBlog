import React, { useEffect, useState, useRef } from "react";
import { FaChevronLeft, FaChevronRight, FaReply } from "react-icons/fa";
import "./PostDetail.css";
import { PostResponse } from "../../model/Post";
import { getTimeAgo } from "../../hooks/useTime";
import { useSignalR } from "../../hooks/useSignalR";
import { CommentResponse } from "../../model/Comment";
import * as signalR from "@microsoft/signalr";

interface PostDetailProps {
  postResponse: PostResponse;
  onClose: () => void;
  onEditComment?: (commentId: string, newContent: string) => void;
  onAddComment?: (content: string) => void;
  onAddReply?: (parentId: string, replyContent: string) => void;
}

const PostDetail: React.FC<PostDetailProps> = ({
  postResponse,
  onClose,
  onAddComment,
  onAddReply,
}) => {
  const hubUrl = "https://localhost:5000/commentHub";
  const connection = useSignalR(hubUrl);
  const [currentImageIndex, setCurrentImageIndex] = useState(0);
  const [commentContent, setCommentContent] = useState("");
  const commentInputRef = useRef<HTMLInputElement>(null);
  const [post, setPost] = useState<PostResponse>(postResponse);
  const [showReplies, setShowReplies] = useState<Record<string, boolean>>({});
  const [replyingTo, setReplyingTo] = useState<{
    parentId: string | null;
    username: string;
  }>({
    parentId: null,
    username: "",
  });

  const handleReplyClick = (parentId: string, username: string) => {
    setReplyingTo({ parentId, username });
    setCommentContent(`@${username} `);

    setTimeout(() => {
      document.getElementById("comment-input")?.focus();
    }, 100);
  };

  const handleCommentSubmit = () => {
    if (commentContent.trim()) {
      if (replyingTo.parentId) {
        onAddReply?.(replyingTo.parentId, commentContent);
      } else {
        onAddComment?.(commentContent);
      }
      setCommentContent("");
      setReplyingTo({ parentId: null, username: "" });
    }
  };
  useEffect(() => {
    document.body.classList.add("modal-open");
    return () => {
      document.body.classList.remove("modal-open");
    };
  }, []);
  const handleToggleReplies = (commentId: string) => {
    setShowReplies((prevState) => ({
      ...prevState,
      [commentId]: !prevState[commentId],
    }));
  };

  useEffect(() => {
    if (!connection) {
      return;
    }

    if (connection.state === signalR.HubConnectionState.Connected) {
      connection
        .invoke("JoinPostGroup", post.id)
        .then(() => console.log(`✅ Đã gửi yêu cầu tham gia nhóm: ${post.id}`))
        .catch(console.error);

      connection.on("ReceiveComment", (newComment: CommentResponse) => {
        if (newComment.parentId) {
          postResponse.commentsResponse = postResponse.commentsResponse.map(
            (comment) =>
              comment.commentId === newComment.parentId &&
              newComment.parentId == null
                ? {
                    ...comment,
                    replies: [...comment.replies, newComment].sort(
                      (a, b) =>
                        new Date(b.dateCreate).getTime() -
                        new Date(a.dateCreate).getTime()
                    ),
                  }
                : comment
          );
        } else {
          setPost((prev) => ({
            ...prev,
            commentsResponse: [newComment, ...prev.commentsResponse],
          }));
        }
      });
    }

    return () => {
      if (connection.state === signalR.HubConnectionState.Connected) {
        console.log("🚪 Rời nhóm:", post.id);
        connection.invoke("LeavePostGroup", post.id).catch(console.error);
      }
    };
  }, [connection, post.id]);

  return (
    <div className="post-detail-overlay" onClick={onClose}>
      <div
        className="post-detail-container"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="post-detail-image">
          <img src={post.imageUrls[currentImageIndex]} alt="Post Image" />
          {post.imageUrls.length > 1 && (
            <>
              {currentImageIndex > 0 && (
                <button
                  className="image-nav-button prev"
                  onClick={() => setCurrentImageIndex((prev) => prev - 1)}
                >
                  <FaChevronLeft />
                </button>
              )}
              {currentImageIndex < post.imageUrls.length - 1 && (
                <button
                  className="image-nav-button next"
                  onClick={() => setCurrentImageIndex((prev) => prev + 1)}
                >
                  <FaChevronRight />
                </button>
              )}
            </>
          )}
        </div>

        <div className="post-detail-content">
          <div className="post-detail-header">
            <img
              src={post.author.avatar}
              alt={post.author.fullName}
              className="author-avatar"
            />
            <span className="author-name">{post.author.fullName}</span>
            <div className="post-time">{getTimeAgo(post.dateCreated)}</div>
            <button className="close-button" onClick={onClose}>
              ×
            </button>
          </div>
          <div className="post-detail-comments">
            {post.commentsResponse.map((comment) => (
              <div key={comment.commentId} className="comment-item">
                <img
                  src={comment.author.avatar}
                  alt={comment.author.fullName}
                  className="author-avatar"
                />
                <div className="comment-content">
                  <span className="author-name">{comment.author.fullName}</span>
                  <p>{comment.content}</p>
                  <div className="comment-actions">
                    <span className="comment-time">
                      {getTimeAgo(comment.dateCreate)}
                    </span>

                    <button
                      className="reply-button"
                      onClick={() =>
                        handleReplyClick(
                          comment.commentId,
                          comment.author.fullName
                        )
                      }
                    >
                      <FaReply className="reply-icon" />
                      Phản hồi
                    </button>
                    {comment.replies.length > 0 && (
                      <button
                        className="view-replies-button"
                        onClick={() => handleToggleReplies(comment.commentId)}
                      >
                        {showReplies[comment.commentId]
                          ? "Ẩn phản hồi"
                          : `Xem ${comment.replies.length} phản hồi`}
                      </button>
                    )}
                  </div>

                  {showReplies[comment.commentId] && (
                    <div className="replies-section">
                      <div className="reply-line" />
                      {comment.replies.map((reply) => (
                        <div key={reply.commentId} className="comment-item">
                          <img
                            src={comment.author.avatar}
                            alt={comment.author.fullName}
                            className="author-avatar"
                          />

                          <div className="comment-content">
                            <span className="author-name">
                              {reply.author.fullName}
                            </span>
                            <p>{reply.content}</p>
                            <div className="comment-actions">
                              <span className="comment-time">
                                {getTimeAgo(post.dateCreated)}
                              </span>

                              <button
                                className="reply-button"
                                onClick={() =>
                                  handleReplyClick(
                                    comment.commentId,
                                    comment.author.fullName
                                  )
                                }
                              >
                                <FaReply className="reply-icon" />
                                Phản hồi
                              </button>
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              </div>
            ))}
          </div>

          <div className="post-detail-comment-input">
            <input
              ref={commentInputRef}
              type="text"
              placeholder="Thêm bình luận..."
              value={commentContent}
              onChange={(e) => setCommentContent(e.target.value)}
              onKeyPress={(e) => e.key === "Enter" && handleCommentSubmit()}
              className="comment-input"
            />
            <button
              className="post-button"
              disabled={!commentContent.trim()}
              onClick={handleCommentSubmit}
            >
              Đăng
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PostDetail;
