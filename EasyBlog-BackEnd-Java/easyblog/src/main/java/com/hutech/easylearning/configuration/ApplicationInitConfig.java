package com.hutech.easylearning.configuration;

import com.hutech.easylearning.entity.Role;
import com.hutech.easylearning.entity.User;
import com.hutech.easylearning.repository.RoleRepository;
import com.hutech.easylearning.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.ApplicationRunner;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.crypto.password.PasswordEncoder;

import java.time.LocalDateTime;
import java.util.HashSet;
import java.util.List;

@Configuration
@RequiredArgsConstructor
@Slf4j
public class ApplicationInitConfig {

    @Autowired
    private PasswordEncoder passwordEncoder;

    @Autowired
    private RoleRepository roleRepository;

    @Bean
    ApplicationRunner applicationRunner(UserRepository userRepository) {
        return args -> {
            // Tạo các vai trò "ADMIN" và "USER" nếu chúng chưa tồn tại

            List<String> defaultRoleNames = List.of("ADMIN", "USER");
            for (String roleName : defaultRoleNames) {
                if (roleRepository.findByName(roleName).isEmpty()) {
                    String description = roleName.equals("ADMIN") ? "ROLE_ADMINISTRATOR" : "ROLE_USER";
                    Role role = Role.builder()
                            .name(roleName)
                            .description(description)
                            .dateCreate(LocalDateTime.now())
                            .dateChange(LocalDateTime.now())
                            .changedBy("APPLICATION")
                            .build();
                    roleRepository.save(role);
                    log.info("Role " + roleName + " has been created.");
                }
            }

            if(userRepository.findByUserName("SuperAdmin").isEmpty())
            {
                List<String> defaultRoleADMIN = List.of("ADMIN");
                var roles = roleRepository.findByNameIn(defaultRoleNames);
                User user = User.builder().userName("SuperAdmin")
                        .password(passwordEncoder.encode("Password@1234"))
                        .email("workflowttp@gmail.com")
                        .fullName("Administrator")
                        .dateCreate(LocalDateTime.now())
                        .dateChange(LocalDateTime.now())
                        .changedBy("APPLICATION")
                        .roles(new HashSet<>(roles))
                        .build();
                userRepository.save(user);
                log.warn("admin user has been created with default password: admin, please change it");
            }
        };
    }
}
