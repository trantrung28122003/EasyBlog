import React, { useState, useEffect } from "react";
import ClientShared from "../Shared/ClientShared";
import { DoCallAPIWithToken } from "../../../services/HttpService";
import { GET_USER_INFO_URL, UPDATE_PROFILE_USER } from "../../../constants/API";
import { HTTP_OK } from "../../../constants/HTTPCode";
import DataLoader from "../../../components/lazyLoadComponent/DataLoader";
import ChangePasswordModal from "./component/ChangePasswordModal";

interface UserProfileViewModel {
  imageUrl: string;
  userName: string;
  fullName: string;
  email: string;
  dayOfBirth: string;
}

export interface UpdateProfileRequest {
  fullName: string;
  file: File | null;
}

const UserSetting: React.FC = () => {
  const [profileUser, setProfileUser] = useState<UserProfileViewModel | null>(
    null
  );
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [fullName, setFullName] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const loadUserProfile = async () => {
    setIsLoading(true);
    try {
      DoCallAPIWithToken(GET_USER_INFO_URL, "get").then((res) => {
        if (res.status === HTTP_OK) {
          setProfileUser(res.data.result);
          setFullName(res.data.result.fullName);
        } else {
        }
      });
    } catch (error) {
      console.error("Lỗi khi tải thông tin người dùng:", error);
    } finally {
      setIsLoading(false);
    }
  };
  useEffect(() => {
    loadUserProfile();
  }, []);

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      setImageFile(file);
    }
  };

  const handleUpdateProfile = () => {
    setIsLoading(true);
    const formData = new FormData();
    formData.append("fullName", fullName);
    if (imageFile) {
      formData.append("file", imageFile);
    }
    const URL = UPDATE_PROFILE_USER;
    DoCallAPIWithToken(URL, "post", formData)
      .then((res) => {
        if (res.status === HTTP_OK) {
          setFullName(res.data.result.fullName);
        }
      })
      .catch((err) => {
        console.error("Error updating profile:", err);
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  const [isModalVisible, setModalVisible] = useState(false);
  const handleOpenModal = () => {
    setModalVisible(true);
  };

  const handleCloseModal = () => {
    setModalVisible(false);
  };
  return (
    <ClientShared>
      <DataLoader isLoading={isLoading} />
      <div className="container-xl px-4 mt-4">
        <nav className="nav nav-borders">
          <a
            className="nav-link active ms-0"
            href="https://www.bootdey.com/snippets/view/bs5-edit-profile-account-details"
            target="__blank"
            rel="noopener noreferrer"
          >
            Hồ sơ người dùng
          </a>
        </nav>
        <hr className="mt-0 mb-4" />
        <div className="row">
          <div className="col-xl-4">
            <div className="card mb-4 mb-xl-0">
              <div className="card-header">Ảnh đại diện</div>
              <div className="card-body text-center">
                <img
                  className="img-account-profile rounded-circle mb-2"
                  src={
                    imageFile
                      ? URL.createObjectURL(imageFile)
                      : profileUser?.imageUrl
                  }
                  alt="Profile"
                  style={{ width: "40%" }}
                />
                <div className="small font-italic text-muted mb-4">
                  Tệp tin JPG hoặc PNG không được lớn quá 10 MB
                </div>

                {/* Chỉ hiển thị form chọn ảnh khi người dùng ấn nút "Đổi ảnh" */}
                <input
                  className="form-control"
                  type="file"
                  onChange={handleFileChange}
                />
              </div>
            </div>
          </div>

          <div className="col-xl-8">
            <div className="card mb-4">
              <div className="card-header">Thông tin chi tiết tài khoản</div>
              <div className="card-body">
                <div>
                  <div className="mb-3">
                    <label className="small mb-1" htmlFor="inputUsername">
                      Tên đăng nhập (Đây là tên đăng nhập lúc đăng kí - không
                      thể đổi tên được)
                    </label>
                    <input
                      className="form-control"
                      id="inputUsername"
                      type="text"
                      value={profileUser?.userName}
                      readOnly
                    />
                  </div>
                  <div className="row gx-3 mb-3">
                    <div className="col-md-6">
                      <label className="small mb-1" htmlFor="inputFullName">
                        Tên đầy đủ
                      </label>
                      <input
                        className="form-control"
                        id="inputFullName"
                        name="fullName"
                        type="text"
                        placeholder="Enter your full name"
                        value={fullName} // Hiển thị tên người dùng hiện tại
                        onChange={(e) => setFullName(e.target.value)} // Cập nhật fullName khi người dùng thay đổi
                      />
                    </div>
                    <div className="col-md-6">
                      <label className="small mb-1" htmlFor="inputBirthday">
                        Ngày sinh
                      </label>
                      <input
                        className="form-control"
                        id="inputBirthday"
                        name="BirthDate"
                        type="date"
                        defaultValue={profileUser?.dayOfBirth}
                        readOnly
                      />
                    </div>
                  </div>
                  <div className="mb-3">
                    <label className="small mb-1" htmlFor="inputEmail">
                      Địa chỉ Email
                    </label>
                    <input
                      className="form-control"
                      id="inputEmail"
                      name="Email"
                      type="email"
                      defaultValue={profileUser?.email}
                      readOnly
                    />
                  </div>
                  <div className="row gx-3 mb-3">
                    <div
                      className="col-md-6 d-flex align-items-center"
                      style={{ marginTop: "26px" }}
                    >
                      <button
                        className="btn btn-primary"
                        type="button"
                        style={{ borderRadius: "12px" }}
                        onClick={handleOpenModal}
                      >
                        Đổi mật khẩu
                      </button>

                      <ChangePasswordModal
                        isVisible={isModalVisible}
                        onClose={handleCloseModal}
                      />
                    </div>
                  </div>

                  <div className="md-3">
                    <button
                      className="btn btn-primary mt-4"
                      style={{ borderRadius: "12px", width: "100%" }}
                      onClick={handleUpdateProfile}
                    >
                      Cập nhật thông tin mới
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </ClientShared>
  );
};

export default UserSetting;
