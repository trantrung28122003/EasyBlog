package com.hutech.easylearning.service;

import com.hutech.easylearning.dto.reponse.BlogResponse;
import com.hutech.easylearning.dto.reponse.NotificationResponse;
import com.hutech.easylearning.entity.BlogLike;
import com.hutech.easylearning.enums.NotificationType;
import com.hutech.easylearning.repository.BlogLikeRepository;
import com.hutech.easylearning.repository.BlogRepository;
import com.hutech.easylearning.repository.UserRepository;
import lombok.AccessLevel;
import lombok.RequiredArgsConstructor;
import lombok.experimental.FieldDefaults;
import lombok.extern.slf4j.Slf4j;
import org.springframework.cglib.core.Local;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;

@Service
@RequiredArgsConstructor
@FieldDefaults(level = AccessLevel.PRIVATE)
@Slf4j
public class BlogLikeService {


    private final BlogLikeRepository blogLikeRepository;
    private final UserService userService;
    private final BlogRepository blogRepository;
    private final UserRepository userRepository;
    private final SimpMessagingTemplate simpMessagingTemplate;
    private final NotificationService notificationService;

    public BlogResponse likeBlog(String blogId) {
        var blog = blogRepository.findById(blogId).orElseThrow();
        var currentUser = userService.getMyInfo();
        boolean alreadyLiked = blogLikeRepository.existsByBlogIdAndUserId(blogId, currentUser.getId());
        if (!alreadyLiked) {
            BlogLike blogLike = BlogLike.builder()
                    .blogId(blogId)
                    .userId(currentUser.getId())
                    .dateCreate(LocalDateTime.now())
                    .dateChange(LocalDateTime.now())
                    .changedBy(currentUser.getId())
                    .isDeleted(false)
                    .build();
            blogLikeRepository.save(blogLike);
        }
        boolean islike = blogLikeRepository.existsByBlogIdAndUserId(blog.getId(), currentUser.getId());
        int likeCount = blogLikeRepository.countByBlogId(blog.getId());
        var userByBlogId = userRepository.findById(blog.getUserId()).orElseThrow();
        BlogResponse blogResponse = BlogResponse.builder()
                .id(blog.getId())
                .likeCount(likeCount)
                .commentCount(blog.getComments().size())
                .content(blog.getContent())
                .userFullName(userByBlogId.getFullName())
                .userAvatarUrl(userByBlogId.getImageUrl())
                .imageUrl(blog.getImageUrl())
                .dateCreate(blog.getDateCreate())
                .statusLikeByUser(islike)
                .build();
        String destination ="/topic/blog/like";
        try {
            simpMessagingTemplate.convertAndSend(destination, blogResponse);
        } catch (Exception e) {
            System.err.println("Không thể gửi qua WebSocket: " + e.getMessage());
        }
        notificationService.addNotificationByLikeBlog(blogId);
        return blogResponse;
    }

    @Transactional
    public BlogResponse unlikeBlog(String blogId) {
        var blog = blogRepository.findById(blogId).orElseThrow();
        var currentUser = userService.getMyInfo();
        boolean alreadyLiked = blogLikeRepository.existsByBlogIdAndUserId(blogId, currentUser.getId());
        if (alreadyLiked) {
            blogLikeRepository.deleteByBlogIdAndUserId(blogId, currentUser.getId());
        }
        boolean islike = blogLikeRepository.existsByBlogIdAndUserId(blog.getId(), currentUser.getId());
        int likeCount = blogLikeRepository.countByBlogId(blog.getId());
        var userByBlogId = userRepository.findById(blog.getUserId()).orElseThrow();
        BlogResponse blogResponse = BlogResponse.builder()
                .id(blog.getId())
                .likeCount(likeCount)
                .commentCount(blog.getComments().size())
                .content(blog.getContent())
                .userFullName(userByBlogId.getFullName())
                .userAvatarUrl(userByBlogId.getImageUrl())
                .imageUrl(blog.getImageUrl())
                .dateCreate(blog.getDateCreate())
                .statusLikeByUser(islike)
                .build();
        simpMessagingTemplate.convertAndSend("/topic/blog/like", blogResponse);
        return blogResponse;
    }

    public boolean isBlogLikedByUser(String blogId) {
        var currentUser = userService.getMyInfo();
        return blogLikeRepository.existsByBlogIdAndUserId(blogId, currentUser.getId());
    }

}
