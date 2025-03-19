package com.hutech.easylearning.repository;

import com.hutech.easylearning.entity.Notification;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface NotificationRepository extends JpaRepository<Notification, String> {
    List<Notification> findAllByUserIdOrderByDateCreateDesc(String userId);
}
