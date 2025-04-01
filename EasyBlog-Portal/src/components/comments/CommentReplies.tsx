import React, { useState } from "react";
import { getTimeAgo } from "../../hooks/useTime";
import "./CommentReplies.css";
import { CommentResponse } from "../../model/Comment";
import { FaEdit } from "react-icons/fa";

interface CommentRepliesProps {
  replies: CommentResponse[];
  onReply: (replyContent: string) => void;
  onEdit: (replyId: string, newContent: string) => void;
  currentUserId: string;
}

const CommentReplies: React.FC<CommentRepliesProps> = ({
  replies,
  onReply,
  onEdit,
  currentUserId,
}) => {
  const [showAllReplies, setShowAllReplies] = useState(false);
  const [replyContent, setReplyContent] = useState("");
  const [editingReplyId, setEditingReplyId] = useState<string | null>(null);
  const [editContent, setEditContent] = useState("");

  const displayedReplies = showAllReplies ? replies : replies.slice(0, 3);
  const hasMoreReplies = replies.length > 3;

  const handleSubmitReply = (e: React.FormEvent) => {
    e.preventDefault();
    if (replyContent.trim()) {
      onReply(replyContent);
      setReplyContent("");
    }
  };

  const handleEditClick = (reply: CommentResponse) => {
    setEditingReplyId(reply.commentId);
    setEditContent(reply.content);
  };

  const handleEditSubmit = (replyId: string) => {
    if (editContent.trim()) {
      onEdit(replyId, editContent);
      setEditingReplyId(null);
      setEditContent("");
    }
  };

  const handleCancelEdit = () => {
    setEditingReplyId(null);
    setEditContent("");
  };

  return (
    <div className="comment-replies">
      <div className="replies-list">
        {displayedReplies.map((reply) => (
          <div key={reply.commentId} className="reply-item">
            <img
              src={reply.author.avatar}
              alt={reply.author.fullName}
              className="reply-avatar"
            />
            <div className="reply-content">
              <div className="reply-header">
                <span className="reply-author">{reply.author.fullName}</span>
                <span className="reply-time">
                  {getTimeAgo(reply.dateCreate)}
                </span>
                {reply.author.id === currentUserId && (
                  <button
                    className="edit-button"
                    onClick={() => handleEditClick(reply)}
                  >
                    <FaEdit />
                  </button>
                )}
              </div>
              {editingReplyId === reply.commentId ? (
                <div className="edit-form">
                  <textarea
                    value={editContent}
                    onChange={(e) => setEditContent(e.target.value)}
                    className="edit-textarea"
                    rows={3}
                  />
                  <div className="edit-actions">
                    <button
                      className="cancel-button"
                      onClick={handleCancelEdit}
                    >
                      Hủy
                    </button>
                    <button
                      className="save-button"
                      onClick={() => handleEditSubmit(reply.commentId)}
                    >
                      Lưu
                    </button>
                  </div>
                </div>
              ) : (
                <p className="reply-text">{reply.content}</p>
              )}
              <div className="reply-actions">
                <button className="action-button">Thích</button>
                <span className="likes-count">123 thích</span>
              </div>
            </div>
          </div>
        ))}
      </div>

      {hasMoreReplies && !showAllReplies && (
        <button
          className="show-more-replies"
          onClick={() => setShowAllReplies(true)}
        >
          Xem thêm {replies.length - 3} phản hồi...
        </button>
      )}

      <form className="reply-form" onSubmit={handleSubmitReply}>
        <input
          type="text"
          placeholder="Viết phản hồi..."
          value={replyContent}
          onChange={(e) => setReplyContent(e.target.value)}
          className="reply-input"
        />
        <button
          type="submit"
          className="reply-submit"
          disabled={!replyContent.trim()}
        >
          Gửi
        </button>
      </form>
    </div>
  );
};

export default CommentReplies;
