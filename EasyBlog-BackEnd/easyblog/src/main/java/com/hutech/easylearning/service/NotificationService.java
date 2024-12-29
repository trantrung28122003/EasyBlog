package com.hutech.easylearning.service;


import com.hutech.easylearning.dto.reponse.NotificationResponse;
import com.hutech.easylearning.dto.request.CommentRequest;
import com.hutech.easylearning.dto.request.ReplyRequest;
import com.hutech.easylearning.entity.Comment;
import com.hutech.easylearning.entity.Notification;
import com.hutech.easylearning.enums.NotificationType;
import com.hutech.easylearning.repository.*;
import jakarta.persistence.EntityNotFoundException;
import lombok.AccessLevel;
import lombok.RequiredArgsConstructor;
import lombok.experimental.FieldDefaults;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

@Service
@RequiredArgsConstructor
@FieldDefaults(level = AccessLevel.PRIVATE)
public class NotificationService {

    final UserService userService;
    final NotificationRepository notificationRepository;
    final CommentRepository commentRepository;
    final UserRepository userRepository;
    final SimpMessagingTemplate simpMessagingTemplate;
    private final BlogRepository blogRepository;
    private final BlogLikeRepository blogLikeRepository;

    public List<NotificationResponse> getAllNotificationByUser () {
        List<NotificationResponse> notificationResponseList = new ArrayList<>();
        var currentUser = userService.getMyInfo();
        List<Notification> notificationsByUser = notificationRepository.findAllByUserIdOrderByDateCreateDesc(currentUser.getId());
        for(Notification notification : notificationsByUser) {
            NotificationResponse notificationResponse = new NotificationResponse().builder()
                    .id(notification.getId())
                    .contentNotification(notification.getContent())
                    .dateCreate(notification.getDateCreate())
                    .isRead(notification.isRead())
                    .targetId(notification.getTargetId())
                    .type(notification.getType())
                    .build();
            notificationResponseList.add(notificationResponse);
        }

        return  notificationResponseList;
    }

    public void addNotificationByComment(String userId, String commentId) {
        var userById = userRepository.findById(userId).orElseThrow();
        Comment comment = commentRepository.findById(commentId)
                .orElseThrow(() -> new EntityNotFoundException("Không tìm thấy bình luận với ID: " + commentId));
        var blogById = blogRepository.findById(comment.getBlogId())
                .orElseThrow(() -> new EntityNotFoundException("Không tìm thấy phần học với ID: " + comment.getBlogId()));
        var userOfBlog = userRepository.findById(blogById.getUserId())
                .orElseThrow(() -> new EntityNotFoundException("Không tìm thấy người dùng với ID: " + blogById.getUserId()));
        var userOfComment = userRepository.findById(comment.getUserId())
                .orElseThrow(() -> new EntityNotFoundException("Không tìm thấy người dùng với ID: " + comment.getUserId()));
        if(!userOfBlog.getId().equals( userById.getId())) {
            var contentNotificationOfUserBlog = userById.getFullName() + "đã bình luận về bài viết của bạn ";
            var notificationResponse = createAndSaveNotification(contentNotificationOfUserBlog, NotificationType.COMMENT, userOfBlog.getId(), blogById.getId());
            sendNotification(userOfBlog.getId(), notificationResponse);
        }
    }

    public void addNotificationByReply(ReplyRequest request) {
        Comment comment = commentRepository.findById(request.getCommentId())
                .orElseThrow(() -> new EntityNotFoundException("Không tìm thấy bình luận với ID: " + request.getCommentId()));
        var blogById = blogRepository.findById(comment.getBlogId())
                .orElseThrow(() -> new EntityNotFoundException("Không tìm thấy phần học với ID: " + comment.getBlogId()));
        var userOfBlog = userRepository.findById(blogById.getUserId())
                .orElseThrow(() -> new EntityNotFoundException("Không tìm thấy người dùng với ID: " + blogById.getUserId()));
        var userOfComment = userRepository.findById(comment.getUserId())
                .orElseThrow(() -> new EntityNotFoundException("Không tìm thấy người dùng với ID: " + comment.getUserId()));
        var currentUserReply = userRepository.findById(request.getCurrentUserId())
                .orElseThrow(() -> new EntityNotFoundException("Không tìm thấy người dùng với ID: " + request.getCurrentUserId()));

        if(request.getParentReplyUserId() != null && !request.getParentReplyUserId().equals(request.getCurrentUserId()))
        {
            var parentReplyUser = userRepository.findById(request.getParentReplyUserId())
                    .orElseThrow(() -> new EntityNotFoundException("Không tìm thấy người dùng với ID: " + request.getParentReplyUserId()));
            var contentNotificationOfParentReply = currentUserReply.getFullName() + "đã nhắc đến bạn trong một bình luận của bạn ";
            var notificationResponse = createAndSaveNotification(contentNotificationOfParentReply, NotificationType.COMMENT, parentReplyUser.getId(), blogById.getId());
            sendNotification(parentReplyUser.getId(), notificationResponse);

        } else if(request.getParentReplyUserId() == null && !userOfComment.getId().equals(request.getCurrentUserId()))
        {
            var contentNotificationOfParentComment = currentUserReply.getFullName() + "đã nhắc đến bạn trong một bình luận của bạn ";
            var notificationResponse = createAndSaveNotification(contentNotificationOfParentComment, NotificationType.COMMENT, userOfComment.getId(), blogById.getId());
            sendNotification(userOfComment.getId(), notificationResponse);
        }


        if(!userOfComment.getId().equals( request.getCurrentUserId()) && !userOfBlog.getId().equals(request.getCurrentUserId())) {
            var contentNotificationOfUserBlog = currentUserReply.getFullName() + "đã bình luận về bài viết của bạn ";
            var notificationResponse = createAndSaveNotification(contentNotificationOfUserBlog, NotificationType.COMMENT, userOfBlog.getId(),blogById.getId());
            sendNotification(userOfBlog.getId(), notificationResponse);
        }
    }

    public void addNotificationByPostBlog(String blogId) {
        var currentUser = userService.getMyInfo();
        var contentNotificationByPostBlog = currentUser.getFullName() + " vừa đăng  bài viết mới kìa! ";
        var userList = userService.getUsers();
        for(var user : userList) {
            if(!user.getId().equals(currentUser.getId())) {
                var notificationResponse = createAndSaveNotification(contentNotificationByPostBlog, NotificationType.COMMENT, user.getId(), blogId);
                sendNotification(user.getId(), notificationResponse);
            }
        }
    }
    public void addNotificationByLikeBlog(String blogId) {
        var blogById = blogRepository.findById(blogId).orElseThrow();
        var currentUser = userService.getMyInfo();
        int likeCount = blogLikeRepository.countByBlogId(blogById.getId());
        var contentNotificationByPostBlog = currentUser.getFullName() + " vừa thích bài viết của bạn kìa ! ";
        if(likeCount > 1)
        {
            contentNotificationByPostBlog = currentUser.getFullName() +" và " + (likeCount > 2 ? likeCount - 1 : likeCount )+" người khác đã thích bài viết của bạn kìaaa";
        }
        if(!currentUser.getId().equals( blogById.getUserId())) {
            var notificationResponse = createAndSaveNotification(contentNotificationByPostBlog, NotificationType.COMMENT, blogById.getUserId(), blogId);
            sendNotification(blogById.getUserId(), notificationResponse);
        }
    }

public NotificationResponse updateStatusIsRead(String notificationId) {
        var notification = notificationRepository.findById(notificationId).orElseThrow();
        notification.setRead(true);
        notificationRepository.save(notification);

        NotificationResponse notificationResponse = new NotificationResponse().builder()
                .id(notification.getId())
                .contentNotification(notification.getContent())
                .dateCreate(notification.getDateCreate())
                .isRead(notification.isRead())
                .type(notification.getType())
                .targetId(notification.getTargetId())
                .build();
        return notificationResponse;
    }

    private NotificationResponse createAndSaveNotification(String content, NotificationType type, String userId, String targetId) {
        Notification notification = Notification.builder()
                .content(content)
                .type(type)
                .isRead(false)
                .userId(userId)
                .dateCreate(LocalDateTime.now())
                .dateChange(LocalDateTime.now())
                .isDeleted(false)
                .targetId(targetId)
                .changedBy("Hệ thống")
                .build();
        notificationRepository.save(notification);
        NotificationResponse notificationResponse = new NotificationResponse().builder()
                .id(notification.getId())
                .contentNotification(notification.getContent())
                .dateCreate(notification.getDateCreate())
                .isRead(notification.isRead())
                .type(notification.getType())
                .targetId(notification.getTargetId())
                .build();
        return notificationResponse;
    }

    private void sendNotification(String userId, NotificationResponse notificationResponse) {
        String destination = "/user/" + userId + "/notifications";
        try {
            simpMessagingTemplate.convertAndSend(destination, notificationResponse);
        } catch (Exception e) {
            System.err.println("Không thể gửi thông báo qua WebSocket: " + e.getMessage());
        }
    }

}
