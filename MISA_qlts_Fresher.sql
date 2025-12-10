-- MySQL dump 10.13  Distrib 8.0.40, for Win64 (x86_64)
--
-- Host: localhost    Database: misa_qlts_fresher
-- ------------------------------------------------------
-- Server version	8.4.3

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `asset`
--

DROP TABLE IF EXISTS `asset`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `asset` (
  `asset_id` char(36) COLLATE utf8mb4_0900_as_ci NOT NULL DEFAULT (uuid()) COMMENT 'Id của Tài sản',
  `asset_code` varchar(20) COLLATE utf8mb4_0900_as_ci NOT NULL COMMENT 'Mã của Tài sản',
  `asset_name` varchar(100) COLLATE utf8mb4_0900_as_ci NOT NULL COMMENT 'Tên của Tài sản',
  `department_id` char(36) COLLATE utf8mb4_0900_as_ci NOT NULL COMMENT 'Id của Bộ phận sử dụng',
  `asset_type_id` char(36) COLLATE utf8mb4_0900_as_ci NOT NULL COMMENT 'Id của Loại sản phẩm cố định',
  `created_date` datetime DEFAULT CURRENT_TIMESTAMP COMMENT 'Ngày mua Tài sản',
  `use_year` decimal(18,4) NOT NULL COMMENT 'Năm sử dụng Tài sản',
  `useful_life` decimal(18,4) NOT NULL COMMENT 'Số năm sử dụng tài sản',
  `tracking_start_year` decimal(18,4) NOT NULL COMMENT 'Năm bắt đầu theo dõi Tài sản',
  `decreciation_rate` decimal(18,4) NOT NULL COMMENT 'Tỉ lệ hao mòn của Tài sản',
  `quantity` decimal(18,4) NOT NULL COMMENT 'Số lượng Tài sản',
  `original_price` decimal(22,4) NOT NULL COMMENT 'Nguyên giá của Tài sản',
  `annual_decreciation` decimal(22,4) NOT NULL COMMENT 'Giá trị hao mòn theo năm của Tài sản',
  PRIMARY KEY (`asset_id`),
  UNIQUE KEY `uix_asset_asset_code` (`asset_code`),
  KEY `FK_asset_department` (`department_id`),
  KEY `FK_asset_asset_type` (`asset_type_id`),
  CONSTRAINT `FK_asset_asset_type` FOREIGN KEY (`asset_type_id`) REFERENCES `asset_type` (`asset_type_id`) ON UPDATE CASCADE,
  CONSTRAINT `FK_asset_department` FOREIGN KEY (`department_id`) REFERENCES `department` (`department_id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_as_ci COMMENT='Bảng tài sản';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `asset`
--

LOCK TABLES `asset` WRITE;
/*!40000 ALTER TABLE `asset` DISABLE KEYS */;
INSERT INTO `asset` VALUES ('046493f2-d7b8-4269-9bde-349ee40b962a','TS00017','jqk update','59a8aa90-d1c7-11f0-b73e-18cc18d2ff36','308d2ce8-d1d6-11f0-b73e-18cc18d2ff36','2025-12-08 04:08:23',2025.0000,15.0000,2025.0000,6.6700,1.0000,222222222.0000,14822222.2074),('2f2084de-56e6-473f-9bca-2df02795a1a6','TS00019','mihh','59a8aa90-d1c7-11f0-b73e-18cc18d2ff36','308d1ca8-d1d6-11f0-b73e-18cc18d2ff36','2025-12-09 09:44:57',2025.0000,20.0000,2025.0000,5.0000,1.0000,20000000.0000,1000000.0000),('30b599a3-6e94-41cb-a593-cf15ae2ef463','TS00013','fadsdefsdfdsa','59a6f4c4-d1c7-11f0-b73e-18cc18d2ff36','308d1ca8-d1d6-11f0-b73e-18cc18d2ff36','2025-12-07 15:04:46',2025.0000,20.0000,2025.0000,5.0000,1.0000,10.0000,0.5000),('3291d24e-21d8-45c0-a3a6-10c6ae4b1617','TS00016','jqk','59a8aa90-d1c7-11f0-b73e-18cc18d2ff36','308d2ce8-d1d6-11f0-b73e-18cc18d2ff36','2025-12-08 04:08:16',2025.0000,15.0000,2025.0000,6.6700,1.0000,222222222.0000,14822222.2074),('32cbafe2-982f-4a96-ad62-dae4b0338c5f','TS00003','Bàn học','59a6f4c4-d1c7-11f0-b73e-18cc18d2ff36','308941ef-d1d6-11f0-b73e-18cc18d2ff36','2025-12-06 11:23:32',2025.0000,80.0000,2025.0000,1.2500,6.0000,1000000.0000,12500.0000),('481bd459-2140-4224-b445-09711579b3b7','TS00002','Túi chanel','59a912ff-d1c7-11f0-b73e-18cc18d2ff36','308d6ff4-d1d6-11f0-b73e-18cc18d2ff36','2025-12-06 11:21:49',2025.0000,4.0000,2025.0000,25.0000,2.0000,10000000.0000,2500000.0000),('5d10cf05-d863-4dd7-8c88-cf70cba1d898','TS00009','Samsung Galaxy S23 Ultra','59a92c76-d1c7-11f0-b73e-18cc18d2ff36','308d1ca8-d1d6-11f0-b73e-18cc18d2ff36','2025-12-07 14:34:23',2025.0000,20.0000,2025.0000,5.0000,2.0000,51000000.0000,2550000.0000),('6c6a8bcf-77bc-4b1e-b08d-34a29fbfe404','TS00018','mihh','59a8aa90-d1c7-11f0-b73e-18cc18d2ff36','308d1ca8-d1d6-11f0-b73e-18cc18d2ff36','2025-12-09 09:38:55',2025.0000,20.0000,2025.0000,5.0000,1.0000,20000000.0000,1000000.0000),('75b96771-ca1e-465f-8e79-752d32b29581','TS00011','bbbbbb ccccc','59a8f45e-d1c7-11f0-b73e-18cc18d2ff36','308941ef-d1d6-11f0-b73e-18cc18d2ff36','2025-12-07 14:59:09',2025.0000,80.0000,2025.0000,1.2500,1.0000,20000000.0000,250000.0000),('86e27dbc-e94f-4728-ae11-a9a96e402922','TS00005','Ô tô LEXUS','59a912ff-d1c7-11f0-b73e-18cc18d2ff36','308d77f1-d1d6-11f0-b73e-18cc18d2ff36','2025-12-06 11:25:36',2025.0000,10.0000,2025.0000,10.0000,1.0000,1200000000.0000,120000000.0000),('8a9233d1-c01c-4c96-b316-88879dec6484','TS00012','aaaa','59a6f4c4-d1c7-11f0-b73e-18cc18d2ff36','308d77f1-d1d6-11f0-b73e-18cc18d2ff36','2025-12-07 14:45:37',2025.0000,10.0000,2025.0000,10.0000,1.0000,10000000.0000,1000000.0000),('959af6fb-6c5e-46f4-8a10-922cc4ba366a','TS00006','Đồng hồ ROLEX','59a8aa90-d1c7-11f0-b73e-18cc18d2ff36','308d5e1d-d1d6-11f0-b73e-18cc18d2ff36','2025-12-06 11:26:18',2025.0000,20.0000,2025.0000,5.0000,1.0000,1500000000.0000,75000000.0000),('99733630-8b98-4397-b04a-85b398e0ffca','TS00007','Iphone 17 Pro Max','59a8f45e-d1c7-11f0-b73e-18cc18d2ff36','308d675f-d1d6-11f0-b73e-18cc18d2ff36','2025-12-07 14:33:16',2025.0000,5.0000,2025.0000,20.0000,1.0000,25000000.0000,5000000.0000),('9ce59e0a-e1da-4afd-b286-fdbf6c1a95e0','TS00004','Xe máy Honda','59a6f4c4-d1c7-11f0-b73e-18cc18d2ff36','308d2ce8-d1d6-11f0-b73e-18cc18d2ff36','2025-12-06 11:24:21',2025.0000,15.0000,2025.0000,6.6700,1.0000,12000000.0000,800400.0000),('a664c892-8e9d-47f0-809c-717cf9088ad9','TS00008','Iphone 17 Pro Max','59a8f45e-d1c7-11f0-b73e-18cc18d2ff36','308d675f-d1d6-11f0-b73e-18cc18d2ff36','2025-12-07 14:33:16',2025.0000,5.0000,2025.0000,20.0000,1.0000,25000000.0000,5000000.0000),('adc61636-5c83-41b6-a120-bf9de5ea975c','TS00015','fadsdefsdfdsa update','59a6f4c4-d1c7-11f0-b73e-18cc18d2ff36','308941ef-d1d6-11f0-b73e-18cc18d2ff36','2025-12-07 15:05:47',2025.0000,80.0000,2025.0000,1.2500,1.0000,10.0000,0.1250),('bdf92720-41e9-4c9a-982f-6008d2182684','TS00014','fadsdefsdfdsa','59a6f4c4-d1c7-11f0-b73e-18cc18d2ff36','308d1ca8-d1d6-11f0-b73e-18cc18d2ff36','2025-12-07 15:04:46',2025.0000,20.0000,2025.0000,5.0000,1.0000,10.0000,0.5000);
/*!40000 ALTER TABLE `asset` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `asset_type`
--

DROP TABLE IF EXISTS `asset_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `asset_type` (
  `asset_type_id` char(36) COLLATE utf8mb4_0900_as_ci NOT NULL DEFAULT (uuid()) COMMENT 'Id Loại tài sản cố định',
  `asset_type_code` varchar(20) COLLATE utf8mb4_0900_as_ci NOT NULL COMMENT 'Mã của Loại tài sản cố định',
  `asset_type_name` varchar(100) COLLATE utf8mb4_0900_as_ci DEFAULT NULL COMMENT 'Tên của Loại tài sản cố định',
  `useful_life` int NOT NULL COMMENT 'Số năm sử dụng của Loại tài sản cố định',
  `recreciation_rate` decimal(18,4) NOT NULL DEFAULT '0.0000' COMMENT 'Tỉ lệ hao mòn của Loại tài sản cố định',
  PRIMARY KEY (`asset_type_id`),
  UNIQUE KEY `uix_asset_type_asset_type_code` (`asset_type_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_as_ci COMMENT='Bảng Loại tài sản cố định';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `asset_type`
--

LOCK TABLES `asset_type` WRITE;
/*!40000 ALTER TABLE `asset_type` DISABLE KEYS */;
INSERT INTO `asset_type` VALUES ('308941ef-d1d6-11f0-b73e-18cc18d2ff36','1','Nhà, công trình xây dựng',80,1.2500),('308d1ca8-d1d6-11f0-b73e-18cc18d2ff36','2','Vật kiến trúc',20,5.0000),('308d2ce8-d1d6-11f0-b73e-18cc18d2ff36','3','Xe ô tô',15,6.6700),('308d5e1d-d1d6-11f0-b73e-18cc18d2ff36','4','Phương tiện vận tải khác (ngoài xe ô tô)',20,5.0000),('308d675f-d1d6-11f0-b73e-18cc18d2ff36','5','Máy móc, thiết bị',5,20.0000),('308d6ff4-d1d6-11f0-b73e-18cc18d2ff36','6','Cây lâu năm, súc vật làm việc và/hoặc cho sản phẩm',4,25.0000),('308d77f1-d1d6-11f0-b73e-18cc18d2ff36','7','Tài sản cố định hữu hình khá',10,10.0000);
/*!40000 ALTER TABLE `asset_type` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `department`
--

DROP TABLE IF EXISTS `department`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `department` (
  `department_id` char(36) COLLATE utf8mb4_0900_as_ci NOT NULL DEFAULT (uuid()) COMMENT 'Id của Bộ phận sử dụng',
  `department_code` varchar(20) COLLATE utf8mb4_0900_as_ci NOT NULL COMMENT 'Mã của Bộ phận sử dụng',
  `department_name` varchar(100) COLLATE utf8mb4_0900_as_ci DEFAULT NULL COMMENT 'Tên của Bộ phận sử dụng',
  PRIMARY KEY (`department_id`),
  UNIQUE KEY `uix_department_department_code` (`department_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_as_ci COMMENT='Bảng Bộ phận sử dụng';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `department`
--

LOCK TABLES `department` WRITE;
/*!40000 ALTER TABLE `department` DISABLE KEYS */;
INSERT INTO `department` VALUES ('59a6f4c4-d1c7-11f0-b73e-18cc18d2ff36','01','Ban Giám hiệu'),('59a8aa90-d1c7-11f0-b73e-18cc18d2ff36','02','Phòng Hành chính - Quản trị'),('59a8f45e-d1c7-11f0-b73e-18cc18d2ff36','03','Phòng Tài vụ'),('59a912ff-d1c7-11f0-b73e-18cc18d2ff36','04','Phòng Đào tạo'),('59a92c76-d1c7-11f0-b73e-18cc18d2ff36','05','Tổ chuyên môn (Giáo viên bộ môn)');
/*!40000 ALTER TABLE `department` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-12-10  8:47:00
