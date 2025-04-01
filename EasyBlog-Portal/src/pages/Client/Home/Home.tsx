import React, { useState, useEffect } from "react";
import { FaCog } from "react-icons/fa";
import "./Home.css";
import ClientShared from "../Shared/ClientShared";
import Post from "../../../components/Post/Post";
import CreatePost from "../../../components/createPost/CreatePost";
import Suggestions from "../../../components/suggestions/TopAuthorsPost";
import { ApplicationResponse } from "../../../model/BaseResponse";
import { DoCallAPIWithToken } from "../../../services/HttpService";
import { PostResponse } from "../../../model/Post";
import { GET_ALL_POST } from "../../../constants/API";
import { HTTP_OK } from "../../../constants/HTTPCode";
import { UserProfile } from "../../../model/User";
import { getUserInfo } from "../../../hooks/useLogin";
import DataLoader from "../../../components/lazyLoadComponent/DataLoader";
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

const Home: React.FC = () => {
  const [currentUserProfile, setCurrentUserProfile] =
    useState<UserProfile | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const handleLike = (postId: string) => {};
  const handleSave = (postId: string) => {};

  const addNewPost = (newPost: PostResponse) => {
    setPosts((prevPosts) => {
      const updatedPosts = [newPost, ...prevPosts];
      return updatedPosts.sort(
        (a, b) =>
          new Date(b.dateCreated).getTime() - new Date(a.dateCreated).getTime()
      );
    });
  };

  const [posts, setPosts] = useState<PostResponse[]>([]);
  const doGetPosts = async () => {
    setIsLoading(true);
    try {
      const res = await DoCallAPIWithToken(GET_ALL_POST, "GET");
      if (res.status === HTTP_OK) {
        const data: ApplicationResponse<PostResponse[]> = res.data;
        if (data.isSuccess) {
          setPosts(data.results);
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
    doGetPosts();
    const userInfo = getUserInfo();
    if (userInfo) {
      setCurrentUserProfile(userInfo);
    }
  }, []);

  return (
    <ClientShared>
      <DataLoader isLoading={isLoading} />
      <div className="home-container">
        <div className="left-sidebar">
          <div className="user-profile">
            <img
              src={currentUserProfile?.avatar}
              alt="User avatar"
              className="user-avatar"
            />
            <div className="user-info">
              <h3 className="user-name">{currentUserProfile?.fullName}</h3>
            </div>
            <button className="settings-button">
              <FaCog />
            </button>
          </div>

          <Suggestions />
        </div>

        <div className="main-content">
          <div className="posts-container">
            {posts.map((post) => (
              <Post
                key={post.id}
                postResponse={post}
                onLike={handleLike}
                onSave={handleSave}
              />
            ))}
          </div>
        </div>
        <div className="right-sidebar">
          <CreatePost addPost={addNewPost} />
        </div>
      </div>
    </ClientShared>
  );
};

export default Home;
