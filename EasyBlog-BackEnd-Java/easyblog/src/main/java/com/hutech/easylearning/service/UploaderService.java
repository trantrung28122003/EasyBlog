    package com.hutech.easylearning.service;

    import com.cloudinary.Cloudinary;
    import com.cloudinary.utils.ObjectUtils;
    import org.springframework.stereotype.Service;
    import org.springframework.web.multipart.MultipartFile;

    import java.io.ByteArrayInputStream;
    import java.io.File;
    import java.io.FileOutputStream;
    import java.io.IOException;
    import java.util.Map;

    @Service
    public class UploaderService {
        private Cloudinary cloudinary = new Cloudinary(ObjectUtils.asMap(
                "cloud_name", "dofr3xzmi",
                "api_key", "895621693982456",
                "api_secret", "N_zS9rda3VO9VbnnYxZ4R-Jcig8"));

        public String uploadFile(MultipartFile gif) {

            System.out.println(cloudinary.toString());
            try {
                File uploadedFile = convertMultiPartToFile(gif);
                var uploadResult = cloudinary.uploader().upload(uploadedFile, ObjectUtils.emptyMap());
                boolean isDeleted = uploadedFile.delete();

                if (isDeleted) {
                    System.out.println("File successfully deleted");
                } else
                    System.out.println("File doesn't exist");
                return uploadResult.get("url").toString();
            } catch (Exception e) {
                throw new RuntimeException(e);
            }
        }

        public File convertMultiPartToFile(MultipartFile file) throws IOException {
            File convFile = new File(file.getOriginalFilename());
            FileOutputStream fos = new FileOutputStream(convFile);
            fos.write(file.getBytes());
            fos.close();
            return convFile;
        }


        public String uploadPdfToCloud(byte[] pdfData) {
            try {
                Map<String, Object> uploadResult = cloudinary.uploader().upload(pdfData, ObjectUtils.asMap("resource_type", "auto"));
                return (String) uploadResult.get("url");
            } catch (IOException e) {
                e.printStackTrace();
                return null;
            }
        }
    }
