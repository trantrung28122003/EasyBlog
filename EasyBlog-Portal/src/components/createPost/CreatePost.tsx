import React, { useState, useRef } from "react";
import { FaImage } from "react-icons/fa";
import "./CreatePost.css";
import { DoCallAPIWithToken } from "../../services/HttpService";
import { HTTP_OK } from "../../constants/HTTPCode";
import { toast } from "react-toastify";
import { PostResponse } from "../../model/Post";
import { ApplicationResponse } from "../../model/BaseResponse";
import DataLoader from "../lazyLoadComponent/DataLoader";
import { ADD_POST } from "../../constants/API";

interface CreatePostProps {
  addPost: (newPost: PostResponse) => void;
}

const CreatePost: React.FC<CreatePostProps> = ({ addPost }) => {
  const [postContent, setPostContent] = useState("");
  const [selectedImages, setSelectedImages] = useState<File[]>([]);
  const [imageUrls, setImageUrls] = useState<string[]>([]);
  const [error, setError] = useState("");
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleImageSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files) {
      const newFiles = Array.from(files);
      setSelectedImages((prev) => [...prev, ...newFiles]);
      const newUrls = newFiles.map((file) => URL.createObjectURL(file));
      setImageUrls((prev) => [...prev, ...newUrls]);
    }
  };

  const removeImage = (index: number) => {
    setSelectedImages((prev) => prev.filter((_, i) => i !== index));
    setImageUrls((prev) => {
      URL.revokeObjectURL(prev[index]);
      return prev.filter((_, i) => i !== index);
    });
  };

  const handleCreatePost = async () => {
    try {
      if (selectedImages.length === 0) {
        setError("Vui lòng chọn ít nhất một ảnh");
        return;
      }
      setIsLoading(true);
      const formData = new FormData();
      formData.append("content", postContent);
      selectedImages.forEach((image) => {
        formData.append("files", image);
      });
      const response = await DoCallAPIWithToken(ADD_POST, "POST", formData);
      if (response.status === HTTP_OK) {
        const data: ApplicationResponse<PostResponse> = response.data;
        if (data.isSuccess) {
          addPost(data.results);
          setPostContent("");
          setSelectedImages([]);
          setImageUrls([]);
          setError("");
          toast.success("Tạo bài viết thành công!");
        } else {
          console.error("Lỗi khi tạo bài viết:", data.errors);
        }
      } else {
        setError("Có lỗi xảy ra khi tạo bài viết");
      }
    } catch (error) {
      console.error("Lỗi khi gọi API:", error);
      setError("Có lỗi xảy ra khi tạo bài viết");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      <DataLoader isLoading={isLoading}></DataLoader>
      <div className="create-post">
        <div className="create-post-header">
          <h2>Tạo bài viết mới</h2>
          <button
            className="publish-button"
            onClick={handleCreatePost}
            disabled={selectedImages.length === 0 || isLoading}
          >
            {isLoading ? "Đang đăng..." : "Đăng"}
          </button>
        </div>
        <div className="create-post-content">
          <textarea
            ref={textareaRef}
            className="post-textarea"
            placeholder="Bạn đang nghĩ gì?"
            value={postContent}
            onChange={(e) => setPostContent(e.target.value)}
            disabled={isLoading}
          />

          {error && <div className="error-message">{error}</div>}

          {imageUrls.length > 0 && (
            <div className="selected-images">
              {imageUrls.map((url, index) => (
                <div key={index} className="image-preview">
                  <img src={url} alt={`Preview ${index + 1}`} />
                  <button
                    className="remove-image"
                    onClick={() => removeImage(index)}
                    disabled={isLoading}
                  >
                    ×
                  </button>
                </div>
              ))}
            </div>
          )}

          <div className="post-actions-bar">
            <div className="action-buttons">
              <label className="post-action-button" htmlFor="image-upload">
                <FaImage /> Chọn ảnh
                <input
                  type="file"
                  id="image-upload"
                  multiple
                  accept="image/*"
                  onChange={handleImageSelect}
                  style={{ display: "none" }}
                  disabled={isLoading}
                />
              </label>
            </div>
            <div className="post-action-note">
              <strong>Lưu ý:</strong> Chọn ảnh để chia sẻ khoảnh khắc của bạn.
              Hỗ trợ định dạng JPG, PNG.
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default CreatePost;
