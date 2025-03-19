package com.hutech.easylearning.configuration;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.http.HttpMethod;
import org.springframework.security.config.annotation.method.configuration.EnableMethodSecurity;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.security.oauth2.server.resource.authentication.JwtAuthenticationConverter;
import org.springframework.security.oauth2.server.resource.authentication.JwtGrantedAuthoritiesConverter;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.security.config.annotation.web.configurers.AbstractHttpConfigurer;
import org.springframework.web.cors.CorsConfiguration;
import org.springframework.web.servlet.config.annotation.CorsRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;

import java.util.Arrays;
import java.util.List;

@Configuration
@EnableWebSecurity
@EnableMethodSecurity
public class SecurityConfig {


    private final String[] PUBLIC_POST_ENDPOINTS = {"/users",
            "/auth/login",
            "/auth/introspect",
            "/auth/register",
            "/auth/loginWithGoogle",
            "/ws/**",
            "/email/sendVerificationCode",
            "/email/verifyCode",
            "customer/resetPassword"
    };

    private final String[] PUBLIC_GET_ENDPOINTS = {
            "/topFourMostRegisteredCourses",
            "/customer/searchCourseByName",
            "/payment/confirmPaymentMomoClient",
            "/getAllCategoryWithCourse",
            "/topFourMostCategory",
            "/detailCourse/{param}",
            "/getFeedbacksByCourseWithoutUser/{param}",
            "/getCoursesByCategory/{param}",
            "/search",
            "/getAllCourse",
            "/ws/**",
            "/getFeedbacksWithFiveRating",
            "/getCourseWithDiscount",
            "/getCourseWithFree"

    };


    private CustomJwtDecoder customJwtDecoder;

    @Bean
    public SecurityFilterChain filterChain(HttpSecurity httpSecurity) throws Exception {
        httpSecurity.authorizeHttpRequests(request ->
                request.requestMatchers(HttpMethod.POST, PUBLIC_POST_ENDPOINTS).permitAll()
                        .requestMatchers(HttpMethod.GET, PUBLIC_GET_ENDPOINTS).permitAll()
                        .anyRequest().authenticated());

        httpSecurity.oauth2ResourceServer(oauth2 ->
                oauth2.jwt(jwtConfigurer -> jwtConfigurer.decoder(customJwtDecoder)
                                .jwtAuthenticationConverter(jwtAuthenticationConverter()))
                        .authenticationEntryPoint(new JwtAuthenticationEntryPoint())
        );

        httpSecurity.csrf(AbstractHttpConfigurer::disable);
//        httpSecurity.cors(cors -> cors.configurationSource(request -> {
//            CorsConfiguration configuration = new CorsConfiguration();
//            configuration.setAllowedOrigins(List.of("*"));
//            configuration.setAllowedMethods(List.of("*"));
//            configuration.setAllowedHeaders(List.of("*"));
//            return configuration;
//        }));

        httpSecurity.cors(cors -> cors.configurationSource(request -> {
            CorsConfiguration configuration = new CorsConfiguration();
            configuration.setAllowedOrigins(List.of("http://localhost:5173"));
            configuration.setAllowedMethods(List.of("*"));
            configuration.setAllowedHeaders(List.of("*"));
            configuration.setAllowCredentials(true); // Hỗ trợ credentials
            return configuration;
        }));
        return httpSecurity.build();
    }



    @Bean
    JwtAuthenticationConverter jwtAuthenticationConverter() {
        JwtGrantedAuthoritiesConverter jwtGrantedAuthoritiesConverter = new JwtGrantedAuthoritiesConverter();
        jwtGrantedAuthoritiesConverter.setAuthorityPrefix("ROLE_");
        JwtAuthenticationConverter jwtAuthenticationConverter = new JwtAuthenticationConverter();
        jwtAuthenticationConverter.setJwtGrantedAuthoritiesConverter(jwtGrantedAuthoritiesConverter);
        return jwtAuthenticationConverter;
    }


    @Bean
    PasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder(10);
    }

}
