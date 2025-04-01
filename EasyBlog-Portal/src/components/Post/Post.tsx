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
import PostDetail from "../PostDetail/PostDetail";
import "./Post.css";
import { PostResponse } from "../../model/Post";
import { DoCallAPIWithToken } from "../../services/HttpService";
import { HTTP_OK } from "../../constants/HTTPCode";
import { UPDATE_COMMENT, ADD_COMMENT } from "../../constants/API";
import { getTimeAgo } from "../../hooks/useTime";
import { ApplicationResponse } from "../../model/BaseResponse";
import { CommentResponse } from "../../model/Comment";
import { ToastContainer, toast } from "react-toastify";
import DataLoader from "../lazyLoadComponent/DataLoader";
interface PostProps {
  postResponse: PostResponse;
  onLike?: (id: string) => void;
  onSave?: (id: string) => void;
  onCommentUpdate?: () => void;
}

const Post: React.FC<PostProps> = ({ postResponse, onLike, onSave }) => {
  const [showDetail, setShowDetail] = useState(false);
  const [currentImageIndex, setCurrentImageIndex] = useState(0);
  const [commentContent, setCommentContent] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const postWithComments = {
    ...postResponse,
    comments:
      postResponse.commentsResponse?.length > 0
        ? postResponse.commentsResponse
        : [],
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
      prev < postResponse.imageUrls?.length - 1 ? prev + 1 : prev
    );
  };

  const handleCreateComment = async (commentContent: string) => {
    if (!commentContent.trim()) {
      toast.error("Vui lòng nhập bình luận!");
      return;
    }
    try {
      const createCommentRequest = {
        postId: postResponse.id,
        content: commentContent,
        parentId: null,
      };
      setIsLoading(true);
      const response = await DoCallAPIWithToken(
        ADD_COMMENT,
        "POST",
        createCommentRequest
      );
      if (response.status === HTTP_OK) {
        const data: ApplicationResponse<CommentResponse> = response.data;
        if (data.isSuccess) {
          setCommentContent("");
          const newComment = data.results;
          postResponse.commentsResponse = [
            newComment,
            ...postResponse.commentsResponse,
          ].sort(
            (a, b) =>
              new Date(b.dateCreate).getTime() -
              new Date(a.dateCreate).getTime()
          );
          setShowDetail(true);
        } else {
          console.error("Lỗi khi tạo bình luận:", data.errors);
        }
      } else {
        toast("Có lỗi xảy ra khi tạo bình luận");
      }
    } catch (error) {
      console.error("Lỗi khi gọi API:", error);
      toast("Có lỗi xảy ra khi tạo bình luận");
    } finally {
      setIsLoading(false);
    }
  };

  const handleCreateReply = async (parentId: string, replyContent: string) => {
    if (!replyContent.trim()) {
      toast.error("Vui lòng nhập bình luận!");
      return;
    }
    try {
      const createCommentRequest = {
        postId: postResponse.id,
        content: replyContent,
        parentId: parentId,
      };
      setIsLoading(true);
      const response = await DoCallAPIWithToken(
        ADD_COMMENT,
        "POST",
        createCommentRequest
      );
      if (response.status === HTTP_OK) {
        const data: ApplicationResponse<CommentResponse> = response.data;
        if (data.isSuccess) {
          setCommentContent("");
          const newComment = data.results;

          if (parentId) {
            postResponse.commentsResponse = postResponse.commentsResponse.map(
              (comment) =>
                comment.commentId === parentId
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
          }

          setShowDetail(true);
        } else {
          console.error("Lỗi khi tạo bình luận:", data.errors);
        }
      } else {
        toast("Có lỗi xảy ra khi tạo bình luận");
      }
    } catch (error) {
      console.error("Lỗi khi gọi API:", error);
      toast("Có lỗi xảy ra khi tạo bình luận");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      <DataLoader
        isLoading={isLoading}
        isOpaque={true}
        message={"Đang tạo bình luận"}
      />
      <div className="post-card">
        <div className="post-header">
          <div className="post-author">
            <img
              src={postResponse.author.avatar}
              alt={postResponse.author.fullName}
              className="author-avatar"
            />
            <span className="author-name">{postResponse.author.fullName}</span>
            <div className="post-time">
              {getTimeAgo(postResponse.dateCreated)}
            </div>
          </div>
          <button className="more-options">•••</button>
        </div>

        <div className="post-image">
          <img
            src={postResponse.imageUrls[currentImageIndex]}
            alt={`Post by ${postResponse.author.fullName}`}
          />

          {/* Navigation Arrows */}
          {postResponse.imageUrls.length > 1 && (
            <>
              {currentImageIndex > 0 && (
                <button
                  className="image-nav-button prev"
                  onClick={handlePrevImage}
                >
                  <FaChevronLeft />
                </button>
              )}
              {currentImageIndex < postResponse.imageUrls.length - 1 && (
                <button
                  className="image-nav-button next"
                  onClick={handleNextImage}
                >
                  <FaChevronRight />
                </button>
              )}

              {/* Image Indicators */}
              <div className="image-indicators">
                {postResponse.imageUrls.map((_, index) => (
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
              className={`action-button ${postResponse.isLiked ? "liked" : ""}`}
              onClick={() => onLike?.(postResponse.id)}
            >
              {postResponse.isLiked ? <FaHeart /> : <FaRegHeart />}
            </button>
            <button className="action-button" onClick={handleCommentClick}>
              <FaComment />
            </button>
            <button className="action-button">
              <FaShare />
            </button>
          </div>
          <button
            className={`action-button ${postResponse.isSaved ? "saved" : ""}`}
            onClick={() => onSave?.(postResponse.id)}
          >
            {postResponse.isSaved ? <FaBookmark /> : <FaRegBookmark />}
          </button>
        </div>

        <div className="post-info">
          <div className="likes-count">{postResponse.likeCount} lượt thích</div>
          <div className="post-content">
            <span className="author-name">{postResponse.author.fullName}</span>
            {postResponse.content}
          </div>
          <div className="comments-count" onClick={handleCommentClick}>
            Xem tất cả {postWithComments.comments.length} bình luận
          </div>
        </div>

        <div className="post-comment">
          <input
            type="text"
            placeholder="Thêm bình luận..."
            className="comment-input"
            value={commentContent}
            onChange={(e) => setCommentContent(e.target.value)}
          />
          <button
            className="post-button"
            disabled={!commentContent}
            onClick={() => handleCreateComment(commentContent)}
          >
            Đăng
          </button>
        </div>
      </div>

      {showDetail && (
        <PostDetail
          postResponse={postResponse}
          onClose={() => setShowDetail(false)}
          onAddComment={handleCreateComment}
          onAddReply={handleCreateReply}
        />
      )}
    </>
  );
};

export default Post;
