import React, { useEffect, useState } from "react";
import "./TopAuthorsPost.css";
import { AuthorResponse } from "../../model/Authentication";
import { DoCallAPIWithToken } from "../../services/HttpService";
import { HTTP_OK } from "../../constants/HTTPCode";
import { ApplicationResponse } from "../../model/BaseResponse";
import { GET_TOP_AUTHORS_POST } from "../../constants/API";

const TopAuthorsPost: React.FC = () => {
  const [authors, setAuthors] = useState<AuthorResponse[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const fetchTopAuthorsPost = async () => {
    setIsLoading(true);
    try {
      const res = await DoCallAPIWithToken(GET_TOP_AUTHORS_POST, "GET");
      if (res.status === HTTP_OK) {
        const data: ApplicationResponse<AuthorResponse[]> = res.data;
        if (data.isSuccess) {
          setAuthors(data.results);
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
  useEffect(() => {
    fetchTopAuthorsPost();
  }, []);
  return (
    <>
      <div className="suggested-users">
        <div className="section-header">
          <h3>Đề xuất cho bạn</h3>
          <button className="see-all">Xem tất cả</button>
        </div>
        <div className="suggested-list">
          {authors.map((author) => (
            <div key={author.id} className="suggested-user">
              <img
                src={author.avatar}
                alt={author.fullName}
                className="suggested-avatar"
              />
              <div className="suggested-info">
                <h4>{author.fullName}</h4>
              </div>
              <button className="follow-button">
                {author.postCount} Bài viết
              </button>
            </div>
          ))}
        </div>
      </div>
    </>
  );
};

export default TopAuthorsPost;
