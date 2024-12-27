package com.hutech.easylearning.service;

import com.hutech.easylearning.dto.reponse.UserResponse;
import com.hutech.easylearning.dto.request.*;
import com.hutech.easylearning.entity.User;
import com.hutech.easylearning.exception.AppException;
import com.hutech.easylearning.exception.ErrorCode;
import com.hutech.easylearning.mapper.UserMapper;
import com.hutech.easylearning.repository.RoleRepository;
import com.hutech.easylearning.repository.UserRepository;
import lombok.AccessLevel;
import lombok.RequiredArgsConstructor;
import lombok.experimental.FieldDefaults;
import lombok.extern.slf4j.Slf4j;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.security.access.prepost.PostAuthorize;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.web.multipart.MultipartFile;

import java.time.LocalDateTime;
import java.util.HashSet;
import java.util.List;

@Service
@RequiredArgsConstructor
@FieldDefaults(level = AccessLevel.PRIVATE)
@Slf4j
public class UserService {
    final UserRepository userRepository;
    final UserMapper userMapper;
    final PasswordEncoder passwordEncoder;
    final RoleRepository roleRepository;
    final UploaderService uploaderService;

    public List<UserResponse> getUsers() {
        log.info("In method getUsers");
        return userRepository.findAll().stream().map(userMapper::toUserResponse).toList();
    }


    @PostAuthorize("returnObject.userName == authentication.name")
    public UserResponse getUserById(String id) {
        return userMapper.toUserResponse(userRepository.findById(id).orElseThrow(() -> new RuntimeException("User not found")));
    }


    public void blockUser(String userId) {
       User userById = userRepository.findById(userId).orElseThrow(() -> new RuntimeException("User not found"));
       userById.setIsDeleted(true);
       userRepository.save(userById);
    }


    public void unblockUser(String userId) {
        User userById = userRepository.findById(userId).orElseThrow(() -> new RuntimeException("User not found"));
        userById.setIsDeleted(false);
        userRepository.save(userById);
    }

    public UserResponse getMyInfo()
    {
        var context = SecurityContextHolder.getContext();
        String userName = context.getAuthentication().getName();
        var user = userRepository.findByUserName(userName).orElseThrow(() -> new AppException(ErrorCode.USER_NOT_EXISTED));
        return userMapper.toUserResponse(user);
    }

    public UserResponse getMyInfoByUserId(String userId)
    {
        var user = userRepository.findById(userId).orElseThrow(() -> new AppException(ErrorCode.USER_NOT_EXISTED));
        return userMapper.toUserResponse(user);
    }

    public UserResponse createUser(UserCreationRequest request, MultipartFile file) {
        User user = userMapper.toUser(request);
        String userImageUrl = "http://res.cloudinary.com/dofr3xzmi/image/upload/v1720255836/aoy4tixw5shd9cxh5ep1.jpg";
        if(file != null)
        {
           userImageUrl = uploaderService.uploadFile(file);
        }
        user.setImageUrl(userImageUrl);
        user.setDateCreate(LocalDateTime.now());
        user.setDateChange(LocalDateTime.now());
        user.setEmail(request.getEmail());
        user.setIsDeleted(false);
        user.setFullName(request.getFullName());
        user.setPassword(passwordEncoder.encode(request.getPassword()));
        if(userRepository.existsByUserName(user.getUserName()))
        {
            throw new AppException(ErrorCode.USER_EXISTED);
        }

        if(userRepository.existsByEmail(user.getEmail()))
        {
            throw new AppException(ErrorCode.USER_EXISTED);
        }

        List<String> defaultRoleNames = List.of("USER");
        var roles = roleRepository.findByNameIn(defaultRoleNames);
        user.setRoles(new HashSet<>(roles));
        user = userRepository.save(user);
        return userMapper.toUserResponse(user);
    }

    public UserResponse updateUser(String userId , UserUpdateRequest request) {
        User getUserById = userRepository.findById(userId).orElseThrow(() -> new RuntimeException("User not found"));
        userMapper.updateUser(getUserById, request);
        getUserById.setPassword(passwordEncoder.encode(request.getPassword()));

        var roles = roleRepository.findByNameIn((request.getRoles()));

        getUserById.setRoles(new HashSet<>(roles));

        return userMapper.toUserResponse(userRepository.save(getUserById));
    }

    public UserResponse updateProfileUser(String fullName,  MultipartFile file) {
        var currentUser = getMyInfo();
        User getUserById = userRepository.findById(currentUser.getId()).orElseThrow(() -> new RuntimeException("User not found"));
        String userImageUrl = getUserById.getImageUrl();
        if(file != null)
        {
            userImageUrl = uploaderService.uploadFile(file);
        }
        getUserById.setFullName(fullName);
        getUserById.setImageUrl(userImageUrl);
        return userMapper.toUserResponse(userRepository.save(getUserById));
    }

    public void deleteUser(String userId) {
        userRepository.deleteById(userId);
    }

    public Boolean changePassword(ChangePasswordRequest request) {
        var currentUser = getMyInfo();
        User user = userRepository.findById(currentUser.getId()).orElseThrow(() -> new RuntimeException("User not found"));
        if (!passwordEncoder.matches(request.getOldPassword(), user.getPassword())) {
            return false;
        }
        user.setPassword(passwordEncoder.encode(request.getNewPassword()));
        userRepository.save(user);
        return true;
    }


    public void resetPassword(ResetPasswordRequest request) {
        var userByEmail = userRepository.findByEmail(request.getEmail()).orElseThrow(() -> new RuntimeException("User not found"));
        User user = userRepository.findById(userByEmail.getId()).orElseThrow(() -> new RuntimeException("User not found"));
        user.setPassword(passwordEncoder.encode(request.getNewPassword()));
        userRepository.save(user);
    }

    public boolean isEmailExist(String email) {
        return userRepository.existsByEmail(email);
    }
}
