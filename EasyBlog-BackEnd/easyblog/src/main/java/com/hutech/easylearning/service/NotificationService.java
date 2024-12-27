package com.hutech.easylearning.service;


import com.hutech.easylearning.dto.reponse.NotificationResponse;
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

    public NotificationResponse addNotificationByComment(ReplyRequest request) {

//        var trainingPart = trainingPartRepository.findById(comment.getTrainingPartId())
//                .orElseThrow(() -> new EntityNotFoundException("Không tìm thấy phần học với ID: " + request.getCommentId()));
//        var course = courseRepository.findById(trainingPart.getCourseId()) .orElseThrow(() -> new EntityNotFoundException("Không tìm thấy phần học với ID: " + trainingPart.getCourseId()));
//
//        var contentNotification = "Có người đã trả lời bình luận của bạn tại phần học " + trainingPart.getTrainingPartName() + " của khóa học "
//                + course.getCourseName();
//        var targetId = "/learning/" + course.getId() + "?trainingPartId=" + trainingPart.getId();
        Comment comment = commentRepository.findById(request.getCommentId())
                .orElseThrow(() -> new EntityNotFoundException("Không tìm thấy bình luận với ID: " + request.getCommentId()));
        var userOfComment = userRepository.findById(comment.getUserId())
                .orElseThrow(() -> new EntityNotFoundException("Không tìm thấy người dùng với ID: " + comment.getUserId()));
        var notificationOfUserId  = request.getParentReplyUserId() != null ? request.getParentReplyUserId() : userOfComment.getId();


       Notification notification = new Notification().builder()

                .content("123213")
                .type(NotificationType.COMMENT)
                .isRead(false)
                .userId(notificationOfUserId)
                .dateCreate(LocalDateTime.now())
                .dateChange(LocalDateTime.now())
                .isDeleted(false)
                .targetId("1212")
                .changedBy("Hệ thống")
                .build();
        notificationRepository.save(notification);

        // Xây dựng Response Notification
        NotificationResponse notificationResponse = new NotificationResponse().builder()
                .id(notification.getId())
                .contentNotification(notification.getContent())
                .dateCreate(notification.getDateCreate())
                .isRead(notification.isRead())
                .type(notification.getType())
                .targetId(notification.getTargetId())
                .build();

        String destination = "/user/" + notificationOfUserId + "/notifications";
        try {
            simpMessagingTemplate.convertAndSend(destination, notificationResponse);
        } catch (Exception e) {
            System.err.println("Không thể gửi thông báo qua WebSocket: " + e.getMessage());
        }
        return notificationResponse;
    }

    public NotificationResponse addNotificationByPurchaseCourse(String courseId) {
        var currentUser = userService.getMyInfo();
//        var course = courseRepository.findById(courseId) .orElseThrow(() -> new EntityNotFoundException("Không tìm thấy khoa học với ID: " + courseId));

//        var contentNotification = course.isFree() ? "Bạn tham gia thành công khóa học "+course.getCourseName()+"miễn phí"  + ". Chúc bạn học tập hiệu quả! "
//                : "Bạn đã mua thành công khóa học "+course.getCourseName() + ". Chúc bạn học tập hiệu quả! ";
//
//        var targetId = course.getCourseType() == CourseType.ONLINE ? "/learning/" + course.getId() : "/schedule";
        Notification notification = new Notification().builder()

                .content("123")
                .type(NotificationType.PAYMENT)
                .isRead(false)
                .userId(currentUser.getId())
                .dateCreate(LocalDateTime.now())
                .dateChange(LocalDateTime.now())
                .isDeleted(false)
                .targetId("1212")
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

}
