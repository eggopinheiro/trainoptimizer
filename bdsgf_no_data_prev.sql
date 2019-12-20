-- MySQL dump 10.13  Distrib 5.6.26, for Win64 (x86_64)
--
-- Host: 192.168.13.124    Database: bdsgf
-- ------------------------------------------------------
-- Server version	5.6.13-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `tbalarms`
--

DROP TABLE IF EXISTS `tbalarms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbalarms` (
  `id` bigint(20) NOT NULL,
  `location` int(11) NOT NULL,
  `element` varchar(20) CHARACTER SET latin1 NOT NULL,
  `description` text CHARACTER SET latin1 NOT NULL,
  `creation_tm` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `ack_tm` timestamp NULL DEFAULT '0000-00-00 00:00:00',
  `end_tm` timestamp NULL DEFAULT '0000-00-00 00:00:00',
  `hist` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`id`),
  KEY `creation_tm` (`creation_tm`),
  KEY `ack_tm` (`ack_tm`),
  KEY `end_tm` (`end_tm`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbapp`
--

DROP TABLE IF EXISTS `tbapp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbapp` (
  `id` int(11) NOT NULL,
  `nome` varchar(100) CHARACTER SET latin1 DEFAULT NULL,
  `pref` varchar(25) CHARACTER SET latin1 DEFAULT NULL,
  `appname` varchar(20) CHARACTER SET latin1 DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbbitmap`
--

DROP TABLE IF EXISTS `tbbitmap`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbbitmap` (
  `branch` varchar(50) CHARACTER SET latin1 NOT NULL,
  `km` smallint(6) NOT NULL,
  `yard` varchar(8) CHARACTER SET latin1 NOT NULL,
  `section` tinyint(4) NOT NULL,
  `remote` tinyint(4) NOT NULL,
  `element` varchar(20) CHARACTER SET latin1 NOT NULL,
  `description` varchar(50) CHARACTER SET latin1 NOT NULL,
  `type` varchar(50) CHARACTER SET latin1 NOT NULL,
  `port` tinyint(4) NOT NULL,
  `flag` tinyint(1) DEFAULT NULL,
  `mask` varchar(8) CHARACTER SET latin1 NOT NULL,
  `encoded` varchar(11) CHARACTER SET latin1 NOT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`km`,`element`,`description`,`type`,`port`,`branch`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbbranch`
--

DROP TABLE IF EXISTS `tbbranch`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbbranch` (
  `id` bigint(20) NOT NULL,
  `name` varchar(50) CHARACTER SET latin1 COLLATE latin1_general_ci NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbcar`
--

DROP TABLE IF EXISTS `tbcar`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbcar` (
  `carname` varchar(3) CHARACTER SET latin1 NOT NULL,
  `length` double NOT NULL,
  PRIMARY KEY (`carname`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbcargps`
--

DROP TABLE IF EXISTS `tbcargps`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbcargps` (
  `carid` varchar(6) NOT NULL,
  `location` int(11) NOT NULL,
  `speed` int(11) DEFAULT NULL,
  `valid` tinyint(4) DEFAULT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`carid`,`location`,`hist`),
  KEY `idxValidHist` (`valid`,`hist`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbcategory`
--

DROP TABLE IF EXISTS `tbcategory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbcategory` (
  `id` bigint(20) NOT NULL,
  `name` varchar(50) CHARACTER SET latin1 NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbcells`
--

DROP TABLE IF EXISTS `tbcells`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbcells` (
  `cell_id` bigint(20) NOT NULL,
  `branch_id` bigint(20) NOT NULL,
  `name` varchar(50) NOT NULL,
  `short_name` varchar(50) NOT NULL,
  `start_coordinate` int(11) NOT NULL,
  `end_coordinate` int(11) NOT NULL,
  `hist` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`cell_id`),
  KEY `FK_cell_branch` (`branch_id`),
  CONSTRAINT `FK_cell_branch` FOREIGN KEY (`branch_id`) REFERENCES `tbbranch` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbcflexconfig`
--

DROP TABLE IF EXISTS `tbcflexconfig`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbcflexconfig` (
  `flag` tinyint(1) NOT NULL DEFAULT '0',
  `hist` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbcflexmsg`
--

DROP TABLE IF EXISTS `tbcflexmsg`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbcflexmsg` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `msg_id` bigint(20) NOT NULL,
  `element_id` bigint(20) NOT NULL,
  `regDate` datetime DEFAULT NULL,
  `trainName` varchar(6) CHARACTER SET latin1 DEFAULT NULL,
  `recvCode` int(11) DEFAULT NULL,
  `hist` datetime NOT NULL,
  `sentMsg` text CHARACTER SET latin1,
  `rcvMsg` text CHARACTER SET latin1,
  `errorMsg` text CHARACTER SET latin1,
  `msgType` int(11) DEFAULT NULL,
  `segment` varchar(20) CHARACTER SET latin1 NOT NULL,
  `cmdused` text CHARACTER SET latin1 NOT NULL,
  `sendDate` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `msgType` (`msgType`),
  KEY `idxcflexmsgid` (`msg_id`),
  KEY `idxcflexmsgelemid` (`element_id`),
  KEY `idxcflexmsgDateName` (`regDate`,`trainName`),
  KEY `idxcflexmsgDateNameSeg` (`regDate`,`trainName`,`segment`),
  KEY `idxcflexmsgNameSeg` (`trainName`,`segment`),
  CONSTRAINT `tbcflexmsg_ibfk_1` FOREIGN KEY (`msgType`) REFERENCES `tbcflexmsgtype` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbcflexmsgtype`
--

DROP TABLE IF EXISTS `tbcflexmsgtype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbcflexmsgtype` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(30) CHARACTER SET latin1 NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbcflextrans`
--

DROP TABLE IF EXISTS `tbcflextrans`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbcflextrans` (
  `segment_mov` varchar(20) CHARACTER SET latin1 NOT NULL,
  `segment_cflex` varchar(12) CHARACTER SET latin1 NOT NULL,
  `sgf_mov` varchar(20) CHARACTER SET latin1 DEFAULT NULL,
  PRIMARY KEY (`segment_mov`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbcflextranslation`
--

DROP TABLE IF EXISTS `tbcflextranslation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbcflextranslation` (
  `segment_mov` varchar(20) CHARACTER SET latin1 NOT NULL,
  `segment_cflex` varchar(12) CHARACTER SET latin1 DEFAULT NULL,
  PRIMARY KEY (`segment_mov`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbcompoqueue`
--

DROP TABLE IF EXISTS `tbcompoqueue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbcompoqueue` (
  `pmt_id` varchar(25) CHARACTER SET latin1 NOT NULL,
  `tipo` varchar(10) CHARACTER SET latin1 NOT NULL,
  `compokey` int(11) NOT NULL,
  `pos` smallint(6) NOT NULL,
  `serie` varchar(3) CHARACTER SET latin1 DEFAULT NULL,
  `peso_ind` double NOT NULL,
  `prefix` varchar(5) CHARACTER SET latin1 NOT NULL,
  `date_hist` datetime NOT NULL,
  PRIMARY KEY (`pmt_id`,`tipo`,`compokey`),
  KEY `idxCompoqueuePrefixDate` (`prefix`,`date_hist`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbconfig`
--

DROP TABLE IF EXISTS `tbconfig`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbconfig` (
  `cflex_queue_manager` varchar(20) CHARACTER SET latin1 NOT NULL,
  `cflex_sgf_send_queue` varchar(30) CHARACTER SET latin1 NOT NULL,
  `cflex_send_active` tinyint(1) NOT NULL DEFAULT '0',
  `smartrain_hist` varchar(20) CHARACTER SET latin1 NOT NULL DEFAULT '',
  `smartrain_hist_bkp` varchar(20) CHARACTER SET latin1 NOT NULL DEFAULT '',
  `cluster_flag` varchar(20) CHARACTER SET latin1 DEFAULT NULL,
  `cluster_flag_hist` datetime DEFAULT NULL,
  `cluster_flag_failover_time` smallint(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbcreateplanqueue`
--

DROP TABLE IF EXISTS `tbcreateplanqueue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbcreateplanqueue` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `pmt_id` varchar(25) NOT NULL,
  `message` varchar(255) NOT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idxcreateplantime` (`hist`),
  KEY `idxcreateplankey` (`pmt_id`)
) ENGINE=InnoDB AUTO_INCREMENT=160430 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbcsimovement`
--

DROP TABLE IF EXISTS `tbcsimovement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbcsimovement` (
  `train_id` bigint(20) NOT NULL,
  `element` varchar(50) CHARACTER SET latin1 NOT NULL,
  `occupation_time` datetime NOT NULL,
  `to_send` bit(1) NOT NULL DEFAULT b'1',
  `os_used` varchar(22) CHARACTER SET latin1 DEFAULT NULL,
  `send_time` datetime DEFAULT NULL,
  `return_time` datetime DEFAULT NULL,
  `return` text CHARACTER SET latin1,
  `retries` int(11) NOT NULL DEFAULT '0',
  `hist` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`train_id`,`element`,`occupation_time`),
  KEY `idx_occupation_time_to_send` (`occupation_time`,`to_send`),
  CONSTRAINT `fk_train_csimovement` FOREIGN KEY (`train_id`) REFERENCES `tbtrain` (`train_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbcsiosmap`
--

DROP TABLE IF EXISTS `tbcsiosmap`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbcsiosmap` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `os_from` varchar(22) CHARACTER SET latin1 NOT NULL DEFAULT '0',
  `os_to` varchar(22) CHARACTER SET latin1 NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `os_from` (`os_from`)
) ENGINE=InnoDB AUTO_INCREMENT=123 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbdefeito`
--

DROP TABLE IF EXISTS `tbdefeito`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbdefeito` (
  `code` varchar(5) CHARACTER SET latin1 NOT NULL DEFAULT '',
  `description` varchar(50) CHARACTER SET latin1 DEFAULT NULL,
  PRIMARY KEY (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbelement`
--

DROP TABLE IF EXISTS `tbelement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbelement` (
  `code` varchar(6) CHARACTER SET latin1 NOT NULL DEFAULT '',
  `name` varchar(50) CHARACTER SET latin1 DEFAULT NULL,
  PRIMARY KEY (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbfalhas`
--

DROP TABLE IF EXISTS `tbfalhas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbfalhas` (
  `numseq` bigint(20) NOT NULL,
  `olocal` varchar(90) CHARACTER SET latin1 DEFAULT NULL,
  `plantonista` varchar(150) CHARACTER SET latin1 DEFAULT NULL,
  `OS` varchar(10) CHARACTER SET latin1 DEFAULT NULL,
  `data_inicial` datetime DEFAULT NULL,
  `falha` text CHARACTER SET latin1,
  `tec_acion` text CHARACTER SET latin1,
  `data_acion` datetime DEFAULT NULL,
  `equipe` varchar(255) CHARACTER SET latin1 DEFAULT NULL,
  `tec_atend` text CHARACTER SET latin1,
  `data_atend` datetime DEFAULT NULL,
  `data_final` datetime DEFAULT NULL,
  `servico` text CHARACTER SET latin1,
  `eqpto` varchar(50) CHARACTER SET latin1 DEFAULT NULL,
  `loco` varchar(50) CHARACTER SET latin1 DEFAULT NULL,
  `trem` varchar(10) CHARACTER SET latin1 DEFAULT NULL,
  `maquinista` varchar(255) CHARACTER SET latin1 DEFAULT NULL,
  `1_loco` varchar(20) CHARACTER SET latin1 DEFAULT NULL,
  `2_loco` varchar(20) CHARACTER SET latin1 DEFAULT NULL,
  `3_loco` varchar(20) CHARACTER SET latin1 DEFAULT NULL,
  `4_loco` varchar(20) CHARACTER SET latin1 DEFAULT NULL,
  `5_loco` varchar(20) CHARACTER SET latin1 DEFAULT NULL,
  `6_loco` varchar(20) CHARACTER SET latin1 DEFAULT NULL,
  `responsavel` varchar(50) CHARACTER SET latin1 DEFAULT NULL,
  `area` varchar(50) CHARACTER SET latin1 DEFAULT NULL,
  `cod` varchar(255) CHARACTER SET latin1 DEFAULT NULL,
  `tempo_impacto` double NOT NULL,
  `sistema` varchar(50) CHARACTER SET latin1 DEFAULT NULL,
  `sede` varchar(5) CHARACTER SET latin1 DEFAULT NULL,
  `isfail` tinyint(1) DEFAULT '0',
  `encerrado` tinyint(1) NOT NULL DEFAULT '0',
  `hist_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `defeito` varchar(50) CHARACTER SET latin1 NOT NULL DEFAULT '',
  `indisp` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`numseq`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbfieldmessage`
--

DROP TABLE IF EXISTS `tbfieldmessage`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbfieldmessage` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `hist` datetime NOT NULL,
  `branch` varchar(15) CHARACTER SET latin1 NOT NULL,
  `km` smallint(6) NOT NULL,
  `element` varchar(20) CHARACTER SET latin1 NOT NULL,
  `description` varchar(40) CHARACTER SET latin1 NOT NULL,
  `is_command` tinyint(1) NOT NULL DEFAULT '0',
  `apy_cmd` tinyint(1) NOT NULL DEFAULT '0',
  `apy_cmd_time` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`id`),
  KEY `idxFieldMessageAll` (`hist`,`branch`,`km`),
  KEY `idxFieldMessageElement` (`hist`,`branch`,`km`,`element`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbhistorico`
--

DROP TABLE IF EXISTS `tbhistorico`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbhistorico` (
  `idtbhistorico` int(11) NOT NULL AUTO_INCREMENT,
  `usuario` varchar(100) CHARACTER SET latin1 DEFAULT NULL,
  `sql_query` varchar(1024) CHARACTER SET latin1 DEFAULT NULL,
  `data` datetime DEFAULT NULL,
  PRIMARY KEY (`idtbhistorico`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbimpactos`
--

DROP TABLE IF EXISTS `tbimpactos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbimpactos` (
  `numseq` bigint(20) NOT NULL,
  `trem_impacto` varchar(10) CHARACTER SET latin1 NOT NULL,
  `local_impacto` varchar(50) CHARACTER SET latin1 NOT NULL,
  `data_prefixo` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `data_impacto` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `tempo` double NOT NULL,
  PRIMARY KEY (`numseq`,`trem_impacto`,`local_impacto`,`data_prefixo`,`data_impacto`),
  CONSTRAINT `tbimpactos_ibfk_1` FOREIGN KEY (`numseq`) REFERENCES `tbfalhas` (`numseq`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbinterdicao`
--

DROP TABLE IF EXISTS `tbinterdicao`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbinterdicao` (
  `ti_id` bigint(20) NOT NULL,
  `branch_id` bigint(20) DEFAULT '-1',
  `start_pos` int(11) NOT NULL,
  `end_pos` int(11) NOT NULL,
  `start_time` datetime NOT NULL,
  `end_time` datetime DEFAULT NULL,
  `field_interdicted` tinyint(1) DEFAULT NULL,
  `ss_name` varchar(20) CHARACTER SET latin1 DEFAULT NULL,
  `status` varchar(20) CHARACTER SET latin1 DEFAULT NULL,
  `reason` varchar(512) CHARACTER SET latin1 DEFAULT NULL,
  `plan_time` datetime DEFAULT NULL,
  `hist` datetime DEFAULT NULL,
  PRIMARY KEY (`ti_id`),
  KEY `idxInterdicaoDateInit` (`start_time`),
  KEY `idxInterdicaoDateEnd` (`end_time`),
  KEY `idxInterdicaoPosInit` (`start_pos`),
  KEY `idxInderdicaoPosEnd` (`end_pos`),
  KEY `fk_interdiction_branch` (`branch_id`),
  CONSTRAINT `fk_interdiction_branch` FOREIGN KEY (`branch_id`) REFERENCES `tbbranch` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbinterdqueue`
--

DROP TABLE IF EXISTS `tbinterdqueue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbinterdqueue` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `rest_id` bigint(20) NOT NULL,
  `start_pos` int(11) NOT NULL,
  `end_pos` int(11) NOT NULL,
  `start_time` datetime NOT NULL,
  `end_time` datetime DEFAULT NULL,
  `status` tinyint(4) NOT NULL,
  `track` varchar(10) CHARACTER SET latin1 DEFAULT NULL,
  `reason` varchar(512) CHARACTER SET latin1 DEFAULT NULL,
  `reason_code` tinyint(4) NOT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idxtrqueue` (`hist`)
) ENGINE=InnoDB AUTO_INCREMENT=30184253 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbinterdqueue_dev`
--

DROP TABLE IF EXISTS `tbinterdqueue_dev`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbinterdqueue_dev` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `rest_id` bigint(20) NOT NULL,
  `start_pos` int(11) NOT NULL,
  `end_pos` int(11) NOT NULL,
  `start_time` datetime NOT NULL,
  `end_time` datetime DEFAULT NULL,
  `processId` bigint(20) NOT NULL,
  `status` tinyint(4) NOT NULL,
  `track` varchar(10) CHARACTER SET latin1 DEFAULT NULL,
  `reason` varchar(512) CHARACTER SET latin1 DEFAULT NULL,
  `reason_code` tinyint(4) NOT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idxtrqueuedev` (`hist`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tblogalarms`
--

DROP TABLE IF EXISTS `tblogalarms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tblogalarms` (
  `id` bigint(20) NOT NULL,
  `data` datetime NOT NULL,
  `info` varchar(255) CHARACTER SET latin1 NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idxLogAlarmData` (`data`),
  KEY `idxLogAlarmInfo` (`data`,`info`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tblogs`
--

DROP TABLE IF EXISTS `tblogs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tblogs` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `data` datetime NOT NULL,
  `datamili` int(11) NOT NULL,
  `info` varchar(255) CHARACTER SET latin1 NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `idxLogInfo` (`data`,`datamili`,`info`),
  KEY `idxLogData` (`data`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tblogsmchcount`
--

DROP TABLE IF EXISTS `tblogsmchcount`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tblogsmchcount` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `data` datetime NOT NULL,
  `mch` varchar(5) CHARACTER SET latin1 NOT NULL,
  `value` int(11) NOT NULL,
  `alarmfail` int(11) NOT NULL DEFAULT '0',
  `alarmsct` int(11) NOT NULL DEFAULT '0',
  `alarmscnt` int(11) NOT NULL DEFAULT '0',
  `rotaind` int(11) NOT NULL DEFAULT '0',
  `rotacmd` int(11) NOT NULL DEFAULT '0',
  `alarmfailevt` int(11) NOT NULL DEFAULT '0',
  `alarmsctevt` int(11) NOT NULL DEFAULT '0',
  `alarmscntevt` int(11) NOT NULL DEFAULT '0',
  `rotasucess` int(11) DEFAULT '0',
  `rotaexpirou` int(11) DEFAULT '0',
  `rotanoreq` int(11) DEFAULT '0',
  `alarmsctpertrain` double NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `idxLogMCHCountNameData` (`data`,`mch`),
  KEY `idxLogMCHCountData` (`data`),
  KEY `idxLogMCHCountName` (`mch`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tblogstd`
--

DROP TABLE IF EXISTS `tblogstd`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tblogstd` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `data` datetime NOT NULL,
  `datamili` int(11) NOT NULL,
  `section` int(11) DEFAULT NULL,
  `remote` int(11) DEFAULT NULL,
  `port` int(11) DEFAULT NULL,
  `value1` tinyint(1) DEFAULT NULL,
  `value2` tinyint(1) DEFAULT NULL,
  `value3` tinyint(1) DEFAULT NULL,
  `value4` tinyint(1) DEFAULT NULL,
  `value5` tinyint(1) DEFAULT NULL,
  `value6` tinyint(1) DEFAULT NULL,
  `value7` tinyint(1) DEFAULT NULL,
  `value8` tinyint(1) DEFAULT NULL,
  `status` varchar(15) CHARACTER SET latin1 NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `idxLogSTDUniInfo` (`data`,`datamili`,`section`,`port`,`remote`,`status`),
  KEY `idxLogSTDData` (`data`),
  KEY `idxLogSTDmInfo` (`data`,`section`,`port`,`remote`,`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbmemprof`
--

DROP TABLE IF EXISTS `tbmemprof`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbmemprof` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `host` varchar(35) NOT NULL,
  `service` varchar(30) NOT NULL,
  `memory_used` double NOT NULL,
  `memory_max` double NOT NULL,
  `hist` datetime NOT NULL,
  `heap_memory_used` double DEFAULT NULL,
  `heap_memory_free` double DEFAULT NULL,
  `non_heap_memory_used` double DEFAULT NULL,
  `non_heap_memory_free` double DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=276626 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbmetatt`
--

DROP TABLE IF EXISTS `tbmetatt`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbmetatt` (
  `olocal` varchar(8) CHARACTER SET latin1 NOT NULL,
  `turno` tinyint(4) NOT NULL,
  `value` double NOT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`olocal`,`turno`),
  UNIQUE KEY `idxMetaTT` (`olocal`,`turno`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbmtm`
--

DROP TABLE IF EXISTS `tbmtm`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbmtm` (
  `record_id` bigint(20) NOT NULL,
  `cod_type` tinyint(4) NOT NULL,
  `cod_status` tinyint(4) NOT NULL,
  `cod_reason` tinyint(4) NOT NULL,
  `reason` varchar(255) CHARACTER SET latin1 NOT NULL,
  `event_description` varchar(100) CHARACTER SET latin1 NOT NULL,
  `planned_date` datetime NOT NULL,
  `initial_date` datetime DEFAULT NULL,
  `finish_date` datetime DEFAULT NULL,
  `duration` int(11) NOT NULL,
  `position_description` varchar(40) CHARACTER SET latin1 NOT NULL,
  `is_planned_event` tinyint(1) DEFAULT NULL,
  `event_id` bigint(20) NOT NULL,
  PRIMARY KEY (`record_id`),
  KEY `cod_status` (`cod_status`),
  KEY `cod_reason` (`cod_reason`),
  KEY `idxPlannedDate` (`planned_date`),
  KEY `idxInitialDate` (`initial_date`),
  CONSTRAINT `tbmtm_ibfk_1` FOREIGN KEY (`cod_status`) REFERENCES `tbmtmstatus` (`id`),
  CONSTRAINT `tbmtm_ibfk_2` FOREIGN KEY (`cod_reason`) REFERENCES `tbmtmreason` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbmtmreason`
--

DROP TABLE IF EXISTS `tbmtmreason`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbmtmreason` (
  `id` tinyint(4) NOT NULL,
  `name` varchar(50) CHARACTER SET latin1 DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbmtmshift`
--

DROP TABLE IF EXISTS `tbmtmshift`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbmtmshift` (
  `shift_id` bigint(20) NOT NULL,
  `cod_type` tinyint(4) NOT NULL,
  `cod_status` tinyint(4) NOT NULL,
  `reason` varchar(50) CHARACTER SET latin1 NOT NULL,
  `description` varchar(100) CHARACTER SET latin1 NOT NULL,
  `position_description` varchar(100) CHARACTER SET latin1 DEFAULT NULL,
  `initial_date_time` datetime NOT NULL,
  `initial_date_time_real` datetime DEFAULT NULL,
  `end_date_time_real` datetime DEFAULT NULL,
  `is_planned` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`shift_id`),
  KEY `cod_status` (`cod_status`),
  KEY `idxInitialDateTime` (`initial_date_time`),
  KEY `idxInitialDateTimeReal` (`initial_date_time_real`),
  CONSTRAINT `tbmtmshift_ibfk_1` FOREIGN KEY (`cod_status`) REFERENCES `tbmtmstatus` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbmtmstatus`
--

DROP TABLE IF EXISTS `tbmtmstatus`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbmtmstatus` (
  `id` tinyint(4) NOT NULL,
  `name` varchar(20) CHARACTER SET latin1 DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbmtmtype`
--

DROP TABLE IF EXISTS `tbmtmtype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbmtmtype` (
  `id` tinyint(4) NOT NULL,
  `name` varchar(25) CHARACTER SET latin1 DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbnotecategtype`
--

DROP TABLE IF EXISTS `tbnotecategtype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbnotecategtype` (
  `categ_id` bigint(20) NOT NULL,
  `descricao` varchar(40) CHARACTER SET latin1 NOT NULL,
  PRIMARY KEY (`categ_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbopttrainmovsegment`
--

DROP TABLE IF EXISTS `tbopttrainmovsegment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbopttrainmovsegment` (
  `train_id` bigint(20) NOT NULL,
  `horario` datetime NOT NULL,
  `coordinate` int(11) NOT NULL,
  `location` smallint(6) NOT NULL,
  `track` tinyint(4) NOT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`train_id`,`horario`,`location`),
  KEY `idxOptTrain` (`train_id`),
  KEY `idxOptTrainHorario` (`train_id`,`horario`),
  KEY `idxOptTrainLocation` (`train_id`,`location`),
  CONSTRAINT `tbopttrainmovsegment_ibfk_1` FOREIGN KEY (`train_id`) REFERENCES `tbtrain` (`train_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbpassageiro`
--

DROP TABLE IF EXISTS `tbpassageiro`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbpassageiro` (
  `mov_type` varchar(20) CHARACTER SET latin1 NOT NULL,
  `patio` tinyint(4) DEFAULT NULL,
  `yard_at` varchar(7) CHARACTER SET latin1 NOT NULL,
  `direction` tinyint(4) NOT NULL,
  `delta_time_min` int(11) NOT NULL,
  `horario` datetime DEFAULT NULL,
  `segment` varchar(7) CHARACTER SET latin1 NOT NULL DEFAULT '',
  PRIMARY KEY (`mov_type`,`yard_at`,`direction`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbpatios`
--

DROP TABLE IF EXISTS `tbpatios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbpatios` (
  `km_id` varchar(12) CHARACTER SET latin1 NOT NULL,
  `patio` varchar(3) CHARACTER SET latin1 NOT NULL,
  PRIMARY KEY (`km_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbpatiosud`
--

DROP TABLE IF EXISTS `tbpatiosud`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbpatiosud` (
  `coordinate` int(11) NOT NULL,
  `patio` varchar(5) CHARACTER SET latin1 NOT NULL,
  PRIMARY KEY (`coordinate`),
  UNIQUE KEY `idxpatiosnameud` (`patio`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbplan`
--

DROP TABLE IF EXISTS `tbplan`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbplan` (
  `plan_id` bigint(20) NOT NULL,
  `train_name` varchar(6) CHARACTER SET latin1 NOT NULL,
  `origem` int(11) NOT NULL,
  `destino` int(11) NOT NULL,
  `departure_time` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `hist` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `pmt_id` varchar(25) CHARACTER SET latin1 DEFAULT NULL,
  `oid` varchar(20) DEFAULT NULL,
  `branch_id` bigint(20) NOT NULL DEFAULT '-1',
  `origin_track` tinyint(4) NOT NULL DEFAULT '1',
  PRIMARY KEY (`plan_id`),
  KEY `idxplanpmtid` (`pmt_id`),
  CONSTRAINT `tbplan_ibfk_1` FOREIGN KEY (`pmt_id`) REFERENCES `tbtrainpmt` (`pmt_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbplanactivities`
--

DROP TABLE IF EXISTS `tbplanactivities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbplanactivities` (
  `train_id` bigint(20) NOT NULL,
  `patio` int(11) NOT NULL,
  `duracao` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`train_id`,`patio`),
  CONSTRAINT `tbplanactivities_ibfk_1` FOREIGN KEY (`train_id`) REFERENCES `tbtrain` (`train_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbplanpoints`
--

DROP TABLE IF EXISTS `tbplanpoints`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbplanpoints` (
  `plan_id` bigint(20) NOT NULL,
  `horario` datetime NOT NULL,
  `segment` varchar(12) CHARACTER SET latin1 NOT NULL,
  `track` varchar(5) CHARACTER SET latin1 NOT NULL,
  `coordinate` int(11) NOT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`plan_id`,`horario`,`segment`),
  KEY `idxplanpointsplanid` (`horario`),
  CONSTRAINT `tbplanpoints_ibfk_1` FOREIGN KEY (`plan_id`) REFERENCES `tbplan` (`plan_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbplanpointspass`
--

DROP TABLE IF EXISTS `tbplanpointspass`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbplanpointspass` (
  `plan_id` bigint(20) NOT NULL,
  `horario` datetime NOT NULL,
  `segment` varchar(8) CHARACTER SET latin1 NOT NULL,
  `track` varchar(5) CHARACTER SET latin1 NOT NULL,
  `coordinate` int(11) NOT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`plan_id`,`horario`,`segment`),
  CONSTRAINT `tbplanpointspass_ibfk_1` FOREIGN KEY (`plan_id`) REFERENCES `tbplan` (`plan_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbplanqueue`
--

DROP TABLE IF EXISTS `tbplanqueue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbplanqueue` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `train_id` bigint(20) NOT NULL,
  `status` tinyint(4) NOT NULL,
  `name` varchar(6) CHARACTER SET latin1 NOT NULL,
  `orig_segment` int(11) NOT NULL,
  `dest_segment` int(11) NOT NULL,
  `prev_partida` datetime NOT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idxtrainplanqueue` (`hist`)
) ENGINE=InnoDB AUTO_INCREMENT=2872322 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbplanqueue_bkp`
--

DROP TABLE IF EXISTS `tbplanqueue_bkp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbplanqueue_bkp` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `train_id` bigint(20) NOT NULL,
  `processId` double NOT NULL,
  `status` tinyint(4) NOT NULL,
  `name` varchar(6) CHARACTER SET latin1 NOT NULL,
  `orig_segment` varchar(7) CHARACTER SET latin1 NOT NULL,
  `dest_segment` varchar(7) CHARACTER SET latin1 NOT NULL,
  `prev_partida` datetime NOT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idxtrainplanqueuebkp` (`hist`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbplansegment`
--

DROP TABLE IF EXISTS `tbplansegment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbplansegment` (
  `train_id` bigint(20) NOT NULL,
  `horario` datetime NOT NULL,
  `location` int(11) NOT NULL,
  `segment` varchar(10) CHARACTER SET latin1 NOT NULL,
  `track` varchar(5) CHARACTER SET latin1 NOT NULL,
  `coordinate` int(11) NOT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`train_id`,`horario`,`location`,`segment`),
  KEY `idxplansegmenthorario` (`horario`),
  CONSTRAINT `tbplansegment_ibfk_1` FOREIGN KEY (`train_id`) REFERENCES `tbtrain` (`train_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbquantcaronpm`
--

DROP TABLE IF EXISTS `tbquantcaronpm`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbquantcaronpm` (
  `num_cars` int(11) NOT NULL,
  `date_hist` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbquantcaronreception`
--

DROP TABLE IF EXISTS `tbquantcaronreception`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbquantcaronreception` (
  `num_cars` int(11) NOT NULL,
  `date_hist` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbremotadisconnect`
--

DROP TABLE IF EXISTS `tbremotadisconnect`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbremotadisconnect` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `data` datetime NOT NULL,
  `patio` smallint(6) NOT NULL,
  `remota` smallint(6) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `idxRemotes` (`data`,`patio`,`remota`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbrepeaters`
--

DROP TABLE IF EXISTS `tbrepeaters`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbrepeaters` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `local` varchar(5) CHARACTER SET latin1 NOT NULL DEFAULT '0',
  `sede` varchar(5) CHARACTER SET latin1 NOT NULL DEFAULT '0',
  `repeater` varchar(8) CHARACTER SET latin1 NOT NULL DEFAULT '0',
  `brgactive` tinyint(1) NOT NULL DEFAULT '0',
  `communicating` tinyint(1) NOT NULL DEFAULT '0',
  `master` tinyint(1) NOT NULL DEFAULT '0',
  `input_power` int(11) NOT NULL DEFAULT '0',
  `forward_power` int(11) NOT NULL DEFAULT '0',
  `reverse_power` int(11) NOT NULL DEFAULT '0',
  `observacao` text CHARACTER SET latin1,
  `hist` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=120 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbrepeatersstatus`
--

DROP TABLE IF EXISTS `tbrepeatersstatus`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbrepeatersstatus` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `repeater_id` int(11) NOT NULL DEFAULT '0',
  `brgactive` tinyint(1) NOT NULL DEFAULT '0',
  `communicating` tinyint(1) NOT NULL DEFAULT '0',
  `master` tinyint(1) NOT NULL DEFAULT '0',
  `input_power` int(11) NOT NULL DEFAULT '0',
  `forward_power` int(11) NOT NULL DEFAULT '0',
  `reverse_power` int(11) NOT NULL DEFAULT '0',
  `hist` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `repeater_id_hist` (`repeater_id`,`hist`),
  CONSTRAINT `fk_repeater_id` FOREIGN KEY (`repeater_id`) REFERENCES `tbrepeaters` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3300281 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbreqloco`
--

DROP TABLE IF EXISTS `tbreqloco`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbreqloco` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `total` smallint(6) NOT NULL DEFAULT '0',
  `sobra` smallint(6) NOT NULL DEFAULT '0',
  `previsto` datetime NOT NULL,
  `realizado` datetime DEFAULT NULL,
  `l1` tinyint(4) DEFAULT NULL,
  `l2` tinyint(4) DEFAULT NULL,
  `l3` tinyint(4) DEFAULT NULL,
  `l4` tinyint(4) DEFAULT NULL,
  `l1disp` datetime DEFAULT NULL,
  `l2disp` datetime DEFAULT NULL,
  `l3disp` datetime DEFAULT NULL,
  `l4disp` datetime DEFAULT NULL,
  `inicio` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbresponsaveis`
--

DROP TABLE IF EXISTS `tbresponsaveis`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbresponsaveis` (
  `nome` varchar(50) CHARACTER SET latin1 NOT NULL,
  PRIMARY KEY (`nome`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbsegment`
--

DROP TABLE IF EXISTS `tbsegment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbsegment` (
  `location` int(11) NOT NULL,
  `segment` varchar(20) CHARACTER SET latin1 NOT NULL,
  `branch_id` bigint(20) DEFAULT NULL,
  `start_coordinate` int(11) NOT NULL,
  `end_coordinate` int(11) NOT NULL,
  `track` smallint(6) NOT NULL DEFAULT '0',
  `allow_same_line_mov` tinyint(1) DEFAULT '0',
  `is_switch` tinyint(1) DEFAULT '0',
  `crossover_side` varchar(10) DEFAULT NULL,
  `hist` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`location`,`segment`),
  KEY `fk_segment_branch` (`branch_id`),
  CONSTRAINT `fk_segment_branch` FOREIGN KEY (`branch_id`) REFERENCES `tbbranch` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbsegmentbkb`
--

DROP TABLE IF EXISTS `tbsegmentbkb`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbsegmentbkb` (
  `location` int(11) NOT NULL,
  `segment` varchar(20) CHARACTER SET latin1 NOT NULL,
  `branch_id` bigint(20) DEFAULT NULL,
  `start_coordinate` int(11) NOT NULL,
  `end_coordinate` int(11) NOT NULL,
  `track` smallint(6) NOT NULL DEFAULT '0',
  `allow_same_line_mov` tinyint(1) DEFAULT '0',
  `is_switch` tinyint(1) DEFAULT '0',
  `hist` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbsheetqueue`
--

DROP TABLE IF EXISTS `tbsheetqueue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbsheetqueue` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `train_id` bigint(20) NOT NULL,
  `processId` bigint(20) NOT NULL,
  `train_name` varchar(5) CHARACTER SET latin1 NOT NULL,
  `train_weight` double NOT NULL,
  `train_len` int(11) NOT NULL,
  `count_cars` int(11) NOT NULL,
  `OS` varchar(22) CHARACTER SET latin1 DEFAULT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idxsheetqueue` (`hist`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbsheetqueue_dev`
--

DROP TABLE IF EXISTS `tbsheetqueue_dev`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbsheetqueue_dev` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `train_id` bigint(20) NOT NULL,
  `processId` bigint(20) NOT NULL,
  `train_name` varchar(5) CHARACTER SET latin1 NOT NULL,
  `train_weight` int(11) NOT NULL,
  `train_len` int(11) NOT NULL,
  `count_cars` int(11) NOT NULL,
  `OS` varchar(22) CHARACTER SET latin1 DEFAULT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idxsheetqueuedev` (`hist`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbspeedrestricted`
--

DROP TABLE IF EXISTS `tbspeedrestricted`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbspeedrestricted` (
  `sra_id` bigint(20) NOT NULL,
  `branch_id` bigint(20) DEFAULT '-1',
  `start_pos` int(11) NOT NULL,
  `end_pos` int(11) NOT NULL,
  `start_time` datetime NOT NULL,
  `end_time` datetime DEFAULT NULL,
  `flag` tinyint(1) DEFAULT NULL,
  `direction` varchar(15) CHARACTER SET latin1 DEFAULT NULL,
  `forward_speed_limit` int(11) DEFAULT NULL,
  `backward_speed_limit` int(11) DEFAULT NULL,
  `over_switch` tinyint(1) NOT NULL,
  `status` varchar(30) CHARACTER SET latin1 DEFAULT NULL,
  `reason` varchar(512) CHARACTER SET latin1 DEFAULT NULL,
  `start_pos_desc` varchar(100) CHARACTER SET latin1 DEFAULT NULL,
  `end_pos_desc` varchar(100) CHARACTER SET latin1 DEFAULT NULL,
  `hist_date` datetime DEFAULT NULL,
  `progressive` tinyint(1) NOT NULL DEFAULT '0',
  `info` varchar(255) CHARACTER SET latin1 NOT NULL DEFAULT '',
  `start_track` varchar(20) CHARACTER SET latin1 NOT NULL DEFAULT '',
  `end_track` varchar(20) CHARACTER SET latin1 NOT NULL DEFAULT '',
  `distrito` smallint(6) DEFAULT NULL,
  PRIMARY KEY (`sra_id`),
  KEY `idxSpeedDateInit` (`start_time`),
  KEY `idxSpeedDateEnd` (`end_time`),
  KEY `idxSpeedPosInit` (`start_pos`),
  KEY `idxSpeedPosEnd` (`end_pos`),
  KEY `distrito` (`distrito`),
  KEY `idxsrdatestatus` (`start_time`,`end_time`,`status`),
  KEY `fk_speedrestricted_branch` (`branch_id`),
  CONSTRAINT `fk_speedrestricted_branch` FOREIGN KEY (`branch_id`) REFERENCES `tbbranch` (`id`),
  CONSTRAINT `tbspeedrestricted_ibfk_1` FOREIGN KEY (`distrito`) REFERENCES `tbsrdistrict` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbsrconfig`
--

DROP TABLE IF EXISTS `tbsrconfig`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbsrconfig` (
  `extensionTrain` double NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbsrdistrict`
--

DROP TABLE IF EXISTS `tbsrdistrict`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbsrdistrict` (
  `id` smallint(6) NOT NULL,
  `description` varchar(40) CHARACTER SET latin1 NOT NULL,
  `name` varchar(5) CHARACTER SET latin1 NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbsrqueue`
--

DROP TABLE IF EXISTS `tbsrqueue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbsrqueue` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `rest_id` bigint(20) NOT NULL,
  `start_pos` int(11) NOT NULL,
  `end_pos` int(11) NOT NULL,
  `start_time` datetime NOT NULL,
  `end_time` datetime DEFAULT NULL,
  `status` tinyint(4) NOT NULL,
  `track` varchar(5) CHARACTER SET latin1 DEFAULT NULL,
  `reason` varchar(512) CHARACTER SET latin1 NOT NULL,
  `reason_code` tinyint(4) NOT NULL,
  `isFlagged` tinyint(1) DEFAULT NULL,
  `speed` int(11) DEFAULT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idxtrqueue` (`hist`)
) ENGINE=InnoDB AUTO_INCREMENT=6259712 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbsrqueue_dev`
--

DROP TABLE IF EXISTS `tbsrqueue_dev`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbsrqueue_dev` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `rest_id` bigint(20) NOT NULL,
  `start_pos` int(11) NOT NULL,
  `end_pos` int(11) NOT NULL,
  `start_time` datetime NOT NULL,
  `end_time` datetime DEFAULT NULL,
  `processId` bigint(20) NOT NULL,
  `status` tinyint(4) NOT NULL,
  `track` varchar(5) CHARACTER SET latin1 DEFAULT NULL,
  `reason` varchar(512) CHARACTER SET latin1 NOT NULL,
  `reason_code` tinyint(4) NOT NULL,
  `isFlagged` tinyint(1) DEFAULT NULL,
  `speed` int(11) DEFAULT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idxtrqueuedev` (`hist`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbstoplocations`
--

DROP TABLE IF EXISTS `tbstoplocations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbstoplocations` (
  `location` int(11) NOT NULL,
  `start_coordinate` int(11) NOT NULL,
  `end_coordinate` int(11) NOT NULL,
  `capacity` tinyint(4) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtfpmcj`
--

DROP TABLE IF EXISTS `tbtfpmcj`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtfpmcj` (
  `linha` varchar(10) CHARACTER SET latin1 NOT NULL,
  `olocal` varchar(3) CHARACTER SET latin1 NOT NULL,
  `total` smallint(6) NOT NULL DEFAULT '0',
  `hist` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`linha`,`olocal`),
  KEY `idxtflinha` (`olocal`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtgparam`
--

DROP TABLE IF EXISTS `tbtgparam`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtgparam` (
  `param_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `trans_time` int(11) NOT NULL,
  `train_type` varchar(1) CHARACTER SET latin1 DEFAULT NULL,
  `date_hist` datetime NOT NULL,
  PRIMARY KEY (`param_id`)
) ENGINE=InnoDB AUTO_INCREMENT=33 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtgsegment`
--

DROP TABLE IF EXISTS `tbtgsegment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtgsegment` (
  `location` int(11) NOT NULL,
  `start_coordinate` int(11) NOT NULL,
  `end_coordinate` int(11) NOT NULL,
  `capacity` tinyint(4) NOT NULL DEFAULT '0',
  PRIMARY KEY (`location`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrain`
--

DROP TABLE IF EXISTS `tbtrain`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrain` (
  `train_id` bigint(20) NOT NULL,
  `name` varchar(20) CHARACTER SET latin1 NOT NULL,
  `creation_tm` datetime NOT NULL,
  `type` varchar(1) DEFAULT NULL,
  `priority` varchar(15) DEFAULT NULL,
  `departure_time` datetime NOT NULL,
  `arrival_time` datetime NOT NULL,
  `direction` smallint(6) DEFAULT NULL,
  `status` varchar(15) CHARACTER SET latin1 DEFAULT NULL,
  `departure_coordinate` int(11) DEFAULT NULL,
  `arrival_coordinate` int(11) DEFAULT NULL,
  `last_coordinate` int(11) DEFAULT '-99999999',
  `last_info_updated` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `pmt_id` varchar(25) CHARACTER SET latin1 DEFAULT NULL,
  `OS` varchar(22) CHARACTER SET latin1 NOT NULL DEFAULT '',
  `plan_id` bigint(20) DEFAULT NULL,
  `OSSGF` varchar(22) CHARACTER SET latin1 DEFAULT NULL,
  `last_track` varchar(8) CHARACTER SET latin1 DEFAULT NULL,
  `hist` datetime DEFAULT NULL,
  `cmd_loco_id` smallint(6) NOT NULL DEFAULT '0',
  `usr_cmd_loco_id` smallint(6) NOT NULL DEFAULT '0',
  `plan_id_lock` tinyint(1) NOT NULL DEFAULT '0',
  `oid` varchar(20) CHARACTER SET latin1 DEFAULT NULL,
  `unilogcurrcoord` int(11) NOT NULL DEFAULT '-99999999',
  `isvalid` int(11) NOT NULL DEFAULT '0',
  `unilogcurinfodate` datetime DEFAULT NULL,
  `unilogcurseg` varchar(12) CHARACTER SET latin1 NOT NULL DEFAULT '',
  `loco_code` bigint(20) NOT NULL DEFAULT '0',
  `lotes` double DEFAULT '3',
  PRIMARY KEY (`train_id`),
  KEY `idxTrainName` (`name`(4)),
  KEY `idxTrainStatus` (`status`),
  KEY `idxTrainDirection` (`direction`),
  KEY `idxTrainDepartureTM` (`departure_time`),
  KEY `idxTrainArrivalTM` (`arrival_time`),
  KEY `plan_id` (`plan_id`),
  CONSTRAINT `tbtrain_ibfk_1` FOREIGN KEY (`plan_id`) REFERENCES `tbplan` (`plan_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainaccuracy`
--

DROP TABLE IF EXISTS `tbtrainaccuracy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainaccuracy` (
  `accuracy_id` bigint(20) NOT NULL,
  `train_id` bigint(20) NOT NULL,
  `plan_arrival` datetime NOT NULL,
  `real_arrival` datetime NOT NULL,
  PRIMARY KEY (`accuracy_id`),
  KEY `train_id` (`train_id`),
  CONSTRAINT `tbtrainaccuracy_ibfk_1` FOREIGN KEY (`train_id`) REFERENCES `tbtrain` (`train_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainactivity`
--

DROP TABLE IF EXISTS `tbtrainactivity`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainactivity` (
  `train_id` bigint(20) NOT NULL DEFAULT '0',
  `branch_id` bigint(20) NOT NULL DEFAULT '-1',
  `coordinate` int(11) NOT NULL,
  `km` smallint(6) NOT NULL,
  `duration` smallint(6) NOT NULL,
  `definition` varchar(5) DEFAULT NULL,
  `hist` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`train_id`,`km`,`coordinate`),
  KEY `fk_trainactivity_branch` (`branch_id`),
  CONSTRAINT `fk_trainactivity_branch` FOREIGN KEY (`branch_id`) REFERENCES `tbbranch` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtraincflexused`
--

DROP TABLE IF EXISTS `tbtraincflexused`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtraincflexused` (
  `train_id` bigint(20) NOT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`train_id`),
  CONSTRAINT `tbtraincflexused_ibfk_1` FOREIGN KEY (`train_id`) REFERENCES `tbtrain` (`train_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtraincompo`
--

DROP TABLE IF EXISTS `tbtraincompo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtraincompo` (
  `pmt_id` varchar(25) CHARACTER SET latin1 NOT NULL,
  `tipo` varchar(10) CHARACTER SET latin1 NOT NULL,
  `compokey` int(11) NOT NULL,
  `os` varchar(22) DEFAULT NULL,
  `pos` smallint(6) NOT NULL,
  `serie` varchar(3) CHARACTER SET latin1 DEFAULT NULL,
  `peso_ind` double NOT NULL,
  `total` smallint(6) NOT NULL DEFAULT '0',
  `train_id` bigint(20) DEFAULT NULL,
  `date_hist` datetime NOT NULL,
  PRIMARY KEY (`pmt_id`,`tipo`,`compokey`),
  KEY `idxcompodatehist` (`date_hist`),
  KEY `idxcompoos` (`os`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtraincreatedlog`
--

DROP TABLE IF EXISTS `tbtraincreatedlog`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtraincreatedlog` (
  `pmt_id` varchar(25) CHARACTER SET latin1 NOT NULL,
  `train_id` bigint(20) NOT NULL,
  `hist` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`pmt_id`),
  KEY `train_id` (`train_id`),
  CONSTRAINT `tbtraincreatedlog_ibfk_1` FOREIGN KEY (`train_id`) REFERENCES `tbtrain` (`train_id`),
  CONSTRAINT `tbtraincreatedlog_ibfk_2` FOREIGN KEY (`pmt_id`) REFERENCES `tbtrainpmt` (`pmt_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainlog`
--

DROP TABLE IF EXISTS `tbtrainlog`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainlog` (
  `data` datetime NOT NULL,
  `prefixo` varchar(5) CHARACTER SET latin1 DEFAULT NULL,
  `cod_place` tinyint(4) NOT NULL DEFAULT '0',
  `patio` smallint(6) NOT NULL,
  `ud` varchar(10) CHARACTER SET latin1 NOT NULL,
  `data_desocup` datetime DEFAULT NULL,
  `train_id` bigint(20) DEFAULT NULL,
  `hist_date` datetime DEFAULT NULL,
  PRIMARY KEY (`data`,`patio`,`ud`),
  UNIQUE KEY `idxtrainLocal` (`train_id`,`prefixo`,`patio`,`ud`),
  UNIQUE KEY `idxtrainLocalCodPlace` (`train_id`,`prefixo`,`patio`,`cod_place`),
  KEY `idxtrainlog` (`prefixo`,`patio`,`ud`),
  KEY `idxtrainlogData` (`prefixo`,`patio`,`ud`,`data`),
  KEY `idxtrainlogDataDesoc` (`prefixo`,`patio`,`ud`,`data_desocup`),
  KEY `idxtrainlogDataAll` (`prefixo`,`patio`,`ud`,`data`,`data_desocup`),
  KEY `idxtrainlogDataAllNoPrefix` (`patio`,`ud`,`data`,`data_desocup`),
  CONSTRAINT `tbtrainlog_ibfk_1` FOREIGN KEY (`train_id`) REFERENCES `tbtrain` (`train_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainmap`
--

DROP TABLE IF EXISTS `tbtrainmap`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainmap` (
  `train_id` bigint(20) NOT NULL,
  `ref_train_id` bigint(20) NOT NULL,
  `hist` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`train_id`),
  KEY `ref_train_id` (`ref_train_id`),
  CONSTRAINT `tbtrainmap_ibfk_1` FOREIGN KEY (`ref_train_id`) REFERENCES `tbtrain` (`train_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainmccomovmsg`
--

DROP TABLE IF EXISTS `tbtrainmccomovmsg`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainmccomovmsg` (
  `patio` smallint(6) NOT NULL,
  `ud` tinyint(4) NOT NULL,
  `trecho` tinyint(4) NOT NULL,
  `data_mov` datetime NOT NULL,
  `trem` varchar(5) CHARACTER SET latin1 NOT NULL,
  `data_pref` datetime NOT NULL,
  `OS` varchar(22) CHARACTER SET latin1 DEFAULT '',
  `km` varchar(8) CHARACTER SET latin1 DEFAULT NULL,
  `isSent` tinyint(1) NOT NULL DEFAULT '0',
  `hist` datetime NOT NULL,
  `segment` varchar(8) CHARACTER SET latin1 NOT NULL DEFAULT '',
  `coordinate` int(11) NOT NULL DEFAULT '-99999999',
  `train_id` bigint(20) NOT NULL,
  PRIMARY KEY (`train_id`,`segment`,`data_mov`),
  KEY `idxtrainmccomovmsg` (`data_mov`),
  KEY `idxtrainmccomovmsgtrem` (`train_id`,`segment`),
  KEY `idxtraimcconmovmsgtrainid` (`train_id`),
  CONSTRAINT `tbtrainmccomovmsg_ibfk_1` FOREIGN KEY (`train_id`) REFERENCES `tbtrain` (`train_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainmov`
--

DROP TABLE IF EXISTS `tbtrainmov`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainmov` (
  `train_id` bigint(20) NOT NULL,
  `horario` datetime NOT NULL,
  `track` varchar(5) CHARACTER SET latin1 NOT NULL,
  `coordinate` int(11) DEFAULT NULL,
  `segment` varchar(20) CHARACTER SET latin1 NOT NULL,
  `hist` datetime DEFAULT NULL,
  `isDup` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`train_id`,`horario`),
  KEY `idxTrainMovId` (`train_id`),
  KEY `idxTrainMovHorario` (`horario`),
  KEY `idxTrainMovCoordinate` (`coordinate`),
  KEY `idxTrainMovRange` (`segment`),
  KEY `idxTrainTrainMovRange` (`train_id`,`segment`),
  KEY `idxTrainMovSegInfo` (`horario`,`segment`),
  CONSTRAINT `tbtrainmov_ibfk_1` FOREIGN KEY (`train_id`) REFERENCES `tbtrain` (`train_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainmovement`
--

DROP TABLE IF EXISTS `tbtrainmovement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainmovement` (
  `train_id` bigint(20) NOT NULL,
  `horario` datetime NOT NULL,
  `track` varchar(15) CHARACTER SET latin1 NOT NULL,
  `coordinate` int(11) DEFAULT NULL,
  `mov_type` varchar(20) CHARACTER SET latin1 NOT NULL DEFAULT '',
  `yard_at` varchar(7) CHARACTER SET latin1 NOT NULL,
  `departed_yard` varchar(7) CHARACTER SET latin1 NOT NULL,
  `cruising_speed` int(11) NOT NULL,
  `stop_time` int(11) DEFAULT NULL,
  `distance` int(11) DEFAULT NULL,
  `ud` varchar(60) CHARACTER SET latin1 NOT NULL,
  `stop_reason_id` smallint(6) DEFAULT NULL,
  `hist` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `sentflag` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`train_id`,`mov_type`,`yard_at`),
  KEY `idxTrainMovementId` (`train_id`),
  KEY `idxTrainHorario` (`horario`),
  KEY `idxTrainCoordinate` (`coordinate`),
  KEY `idxYardAt` (`yard_at`(6)),
  KEY `idxTrainRange` (`yard_at`,`mov_type`),
  KEY `idxMoveType` (`mov_type`),
  KEY `idxTrainMovInfo` (`horario`,`mov_type`,`yard_at`),
  KEY `stop_reason_id` (`stop_reason_id`),
  CONSTRAINT `tbtrainmovement_ibfk_1` FOREIGN KEY (`train_id`) REFERENCES `tbtrain` (`train_id`),
  CONSTRAINT `tbtrainmovement_ibfk_2` FOREIGN KEY (`stop_reason_id`) REFERENCES `tbtrainstopreason` (`reason_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainmovmsg`
--

DROP TABLE IF EXISTS `tbtrainmovmsg`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainmovmsg` (
  `patio` smallint(6) NOT NULL,
  `ud` tinyint(4) NOT NULL,
  `trecho` tinyint(4) NOT NULL,
  `data_mov` datetime NOT NULL,
  `trem` varchar(5) CHARACTER SET latin1 NOT NULL,
  `data_pref` datetime NOT NULL,
  `OS` varchar(22) CHARACTER SET latin1 DEFAULT '',
  `segment` varchar(12) CHARACTER SET latin1 NOT NULL,
  `train_id` bigint(20) NOT NULL,
  `coordinate` int(11) NOT NULL DEFAULT '-99999999',
  `OSSGF` varchar(22) CHARACTER SET latin1 DEFAULT NULL,
  `OSUSD` varchar(22) CHARACTER SET latin1 DEFAULT NULL,
  `placaKM` varchar(4) CHARACTER SET latin1 NOT NULL DEFAULT '',
  `hist` datetime NOT NULL,
  PRIMARY KEY (`train_id`,`patio`,`ud`,`trecho`),
  KEY `idxtrainmovmsg` (`data_mov`),
  KEY `idxtrainmovmsgtrem` (`train_id`,`segment`),
  KEY `idxtrainmovmsgtrainid` (`train_id`),
  CONSTRAINT `tbtrainmovmsg_ibfk_1` FOREIGN KEY (`train_id`) REFERENCES `tbtrain` (`train_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainmovmsgqueue`
--

DROP TABLE IF EXISTS `tbtrainmovmsgqueue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainmovmsgqueue` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `train_id` bigint(20) NOT NULL,
  `horario` datetime NOT NULL,
  `name` varchar(6) CHARACTER SET latin1 NOT NULL,
  `segment` varchar(12) CHARACTER SET latin1 NOT NULL,
  `track` varchar(5) CHARACTER SET latin1 DEFAULT NULL,
  `coordinate` int(11) NOT NULL,
  `patio` smallint(6) NOT NULL,
  `ud` tinyint(4) NOT NULL,
  `trecho` tinyint(4) NOT NULL,
  `data_pref` datetime NOT NULL,
  `OS` varchar(22) CHARACTER SET latin1 DEFAULT '',
  `km` varchar(5) CHARACTER SET latin1 NOT NULL,
  `hist` datetime NOT NULL,
  `info` varchar(10) CHARACTER SET latin1 DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idxtrainmovmsgqueue` (`hist`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainmovmsgs`
--

DROP TABLE IF EXISTS `tbtrainmovmsgs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainmovmsgs` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `trem` varchar(4) NOT NULL,
  `os` varchar(10) NOT NULL,
  `data_prefixo` datetime NOT NULL,
  `patio` varchar(2) NOT NULL,
  `ud` varchar(1) NOT NULL,
  `trecho` varchar(1) NOT NULL,
  `ramal` varchar(2) NOT NULL,
  `km` varchar(4) NOT NULL,
  `data_movimentacao` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `data_geracao` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `os_usada` varchar(10) NOT NULL,
  `train_id` bigint(20) DEFAULT NULL,
  `mensagem` text NOT NULL,
  `hist` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idxdatamovimentacao` (`data_movimentacao`),
  KEY `idxos` (`os`),
  KEY `idxtrem` (`trem`,`data_prefixo`),
  KEY `FK__tbtrain` (`train_id`),
  CONSTRAINT `FK__tbtrain` FOREIGN KEY (`train_id`) REFERENCES `tbtrain` (`train_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainmovqueue`
--

DROP TABLE IF EXISTS `tbtrainmovqueue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainmovqueue` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `train_id` bigint(20) NOT NULL,
  `horario` datetime NOT NULL,
  `name` varchar(6) CHARACTER SET latin1 NOT NULL,
  `segment` varchar(12) CHARACTER SET latin1 NOT NULL,
  `track` varchar(5) CHARACTER SET latin1 DEFAULT NULL,
  `coordinate` int(11) NOT NULL,
  `hist` datetime NOT NULL,
  `info` varchar(10) CHARACTER SET latin1 DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idxtrainmovqueue` (`hist`),
  KEY `idxtrainplanqueue` (`hist`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainmovqueue_dev`
--

DROP TABLE IF EXISTS `tbtrainmovqueue_dev`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainmovqueue_dev` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `train_id` bigint(20) NOT NULL,
  `horario` datetime NOT NULL,
  `name` varchar(6) CHARACTER SET latin1 NOT NULL,
  `segment` varchar(12) CHARACTER SET latin1 NOT NULL,
  `track` varchar(5) CHARACTER SET latin1 DEFAULT NULL,
  `coordinate` int(11) NOT NULL,
  `hist` datetime NOT NULL,
  `info` varchar(10) CHARACTER SET latin1 DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idxtrainmovqueuedev` (`hist`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainmovsegment`
--

DROP TABLE IF EXISTS `tbtrainmovsegment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainmovsegment` (
  `train_id` bigint(20) NOT NULL DEFAULT '0',
  `branch_id` bigint(20) NOT NULL DEFAULT '-1',
  `data_ocup` datetime NOT NULL,
  `data_desocup` datetime DEFAULT NULL,
  `location` smallint(6) NOT NULL,
  `ud` varchar(10) CHARACTER SET latin1 NOT NULL,
  `track` tinyint(4) DEFAULT NULL,
  `coordinate` int(11) NOT NULL,
  `direction` tinyint(4) NOT NULL DEFAULT '0',
  `date_hist` datetime NOT NULL,
  PRIMARY KEY (`train_id`,`data_ocup`,`location`,`ud`),
  KEY `idxTrainMovSegLocation` (`train_id`,`data_ocup`,`location`),
  KEY `idxTrainMovSegLocationD` (`train_id`,`data_desocup`,`location`),
  KEY `fk_brain_trainmovsegment` (`branch_id`),
  KEY `location` (`location`),
  CONSTRAINT `fk_brain_trainmovsegment` FOREIGN KEY (`branch_id`) REFERENCES `tbbranch` (`id`),
  CONSTRAINT `tbtrainmovsegment_ibfk_1` FOREIGN KEY (`train_id`) REFERENCES `tbtrain` (`train_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainmovsegqueue`
--

DROP TABLE IF EXISTS `tbtrainmovsegqueue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainmovsegqueue` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `train_id` bigint(20) NOT NULL,
  `horario` datetime NOT NULL,
  `name` varchar(6) CHARACTER SET latin1 NOT NULL,
  `location` smallint(6) NOT NULL,
  `ud` varchar(10) CHARACTER SET latin1 NOT NULL,
  `track` tinyint(4) DEFAULT NULL,
  `coordinate` int(11) NOT NULL,
  `direction` tinyint(4) NOT NULL DEFAULT '0',
  `hist` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idxtrainmovsegqueue` (`hist`)
) ENGINE=InnoDB AUTO_INCREMENT=46531391 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainmovset`
--

DROP TABLE IF EXISTS `tbtrainmovset`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainmovset` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainmovud`
--

DROP TABLE IF EXISTS `tbtrainmovud`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainmovud` (
  `train_id` bigint(20) NOT NULL DEFAULT '0',
  `data_ocup` datetime NOT NULL,
  `data_desocup` datetime DEFAULT NULL,
  `patio` smallint(6) NOT NULL,
  `ud` varchar(12) CHARACTER SET latin1 NOT NULL,
  `date_hist` datetime NOT NULL,
  `element` varchar(15) CHARACTER SET latin1 DEFAULT NULL,
  PRIMARY KEY (`train_id`,`data_ocup`),
  UNIQUE KEY `idxTrainMovUD` (`train_id`,`data_ocup`,`patio`,`ud`),
  CONSTRAINT `tbtrainmovud_ibfk_1` FOREIGN KEY (`train_id`) REFERENCES `tbtrain` (`train_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainnote`
--

DROP TABLE IF EXISTS `tbtrainnote`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainnote` (
  `note_id` bigint(20) NOT NULL,
  `creation_time` datetime NOT NULL,
  `train_id` bigint(20) NOT NULL,
  `creator` varchar(30) CHARACTER SET latin1 DEFAULT NULL,
  `description` text CHARACTER SET latin1,
  `position_track` varchar(15) CHARACTER SET latin1 DEFAULT NULL,
  `position_coordinate` int(11) NOT NULL,
  `categ_id` bigint(20) NOT NULL DEFAULT '0',
  PRIMARY KEY (`note_id`),
  KEY `idxTrainNoteDate` (`creation_time`),
  KEY `idxTrainNote` (`train_id`),
  KEY `idxTrainNotePosition` (`position_coordinate`),
  KEY `idxTrainNotePositionTrain` (`train_id`,`position_coordinate`),
  CONSTRAINT `tbtrainnote_ibfk_1` FOREIGN KEY (`train_id`) REFERENCES `tbtrain` (`train_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainnotemap`
--

DROP TABLE IF EXISTS `tbtrainnotemap`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainnotemap` (
  `categ_id` bigint(20) NOT NULL,
  `description` varchar(35) CHARACTER SET latin1 NOT NULL,
  PRIMARY KEY (`categ_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainnotequeue`
--

DROP TABLE IF EXISTS `tbtrainnotequeue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainnotequeue` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `train_id` bigint(20) NOT NULL,
  `note_id` bigint(20) NOT NULL,
  `creation_time` datetime NOT NULL,
  `description` text CHARACTER SET latin1 NOT NULL,
  `coordinate` int(11) NOT NULL,
  `hist` datetime NOT NULL,
  `categ_id` bigint(20) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `idxtrainnotequeue` (`hist`)
) ENGINE=InnoDB AUTO_INCREMENT=17043318 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainonline`
--

DROP TABLE IF EXISTS `tbtrainonline`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainonline` (
  `grupo` varchar(15) CHARACTER SET latin1 NOT NULL,
  `horario` datetime NOT NULL,
  `total` double NOT NULL DEFAULT '0',
  PRIMARY KEY (`grupo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainoptdata`
--

DROP TABLE IF EXISTS `tbtrainoptdata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainoptdata` (
  `train_id` bigint(20) NOT NULL,
  `train_name` varchar(6) NOT NULL,
  `timevalue` bigint(20) NOT NULL DEFAULT '0',
  `position` double NOT NULL,
  `hist` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `track` smallint(6) NOT NULL DEFAULT '0',
  `branch_id` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`train_id`,`timevalue`,`position`),
  KEY `idxTrainPlanOptData` (`train_id`),
  KEY `idxTimePlanOptData` (`timevalue`),
  KEY `branch_id` (`branch_id`),
  CONSTRAINT `tbtrainoptdata_ibfk_1` FOREIGN KEY (`branch_id`) REFERENCES `tbbranch` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainoptelapsed`
--

DROP TABLE IF EXISTS `tbtrainoptelapsed`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainoptelapsed` (
  `id` int(11) NOT NULL,
  `start_time` datetime NOT NULL,
  `end_time` datetime DEFAULT NULL,
  `objective_function_called` int(11) NOT NULL DEFAULT '0',
  `generation` int(11) NOT NULL DEFAULT '0',
  `fitness_value` double DEFAULT NULL,
  `objective_function` varchar(10) NOT NULL DEFAULT '',
  `population_count` int(11) NOT NULL DEFAULT '0',
  `hist` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `comment` varchar(255) DEFAULT NULL,
  `initial_fitness` double NOT NULL DEFAULT '0',
  `ls_called_count` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`,`start_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainoptlog`
--

DROP TABLE IF EXISTS `tbtrainoptlog`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainoptlog` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `message` text NOT NULL,
  `source` varchar(255) NOT NULL,
  `stacktrace` text NOT NULL,
  `targetsite` varchar(255) NOT NULL,
  `hist` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=41520728 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainpat`
--

DROP TABLE IF EXISTS `tbtrainpat`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainpat` (
  `pmt_id` varchar(25) CHARACTER SET latin1 NOT NULL,
  `prev_part` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `KMOrigem` smallint(6) NOT NULL DEFAULT '0',
  `KMDestino` smallint(6) NOT NULL DEFAULT '0',
  `KMParada` smallint(6) NOT NULL DEFAULT '0',
  `Activity` varchar(2) CHARACTER SET latin1 NOT NULL,
  `Espera` int(11) NOT NULL DEFAULT '0',
  `original_msg` varchar(256) CHARACTER SET latin1 DEFAULT NULL,
  `date_hist` datetime NOT NULL,
  `plan_id` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`pmt_id`,`KMParada`,`Activity`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainpatqueue`
--

DROP TABLE IF EXISTS `tbtrainpatqueue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainpatqueue` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `train_id` bigint(20) NOT NULL,
  `segment` smallint(6) NOT NULL,
  `activity` varchar(3) CHARACTER SET latin1 NOT NULL,
  `duration` smallint(6) NOT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idxtbtrainpatqueue` (`hist`)
) ENGINE=InnoDB AUTO_INCREMENT=3636233 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainperformance`
--

DROP TABLE IF EXISTS `tbtrainperformance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainperformance` (
  `traintype` varchar(1) NOT NULL,
  `direction` tinyint(4) NOT NULL,
  `location` int(11) NOT NULL,
  `ud` varchar(10) NOT NULL,
  `destination_track` tinyint(4) NOT NULL DEFAULT '1',
  `stop_location` int(11) DEFAULT NULL,
  `timemov` double NOT NULL DEFAULT '0',
  `timestop` double NOT NULL DEFAULT '0',
  `timeheadwaymov` double NOT NULL,
  `timeheadwaystop` double NOT NULL,
  `timemovstop` double NOT NULL DEFAULT '0',
  `timestopstop` double NOT NULL DEFAULT '0',
  `timeheadwaymovstop` double NOT NULL DEFAULT '0',
  `timeheadwaystopstop` double NOT NULL DEFAULT '0',
  `hist` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`traintype`,`direction`,`location`,`ud`,`destination_track`),
  KEY `idxTrainPerformanceStop` (`traintype`,`direction`,`stop_location`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainplan`
--

DROP TABLE IF EXISTS `tbtrainplan`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainplan` (
  `plan_id` bigint(20) NOT NULL,
  `horario` datetime NOT NULL,
  `track` tinyint(4) NOT NULL,
  `coordinate` int(11) DEFAULT NULL,
  `patio` tinyint(4) NOT NULL,
  `yard_at` varchar(7) CHARACTER SET latin1 NOT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`plan_id`,`patio`,`horario`),
  KEY `idxTrainPlanId` (`plan_id`),
  KEY `idxTrainPlanHorario` (`horario`),
  KEY `idxTrainPlanRange` (`patio`,`horario`),
  CONSTRAINT `tbtrainplan_ibfk_1` FOREIGN KEY (`plan_id`) REFERENCES `tbplan` (`plan_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainplansegment`
--

DROP TABLE IF EXISTS `tbtrainplansegment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainplansegment` (
  `train_id` bigint(20) NOT NULL,
  `horario` datetime NOT NULL,
  `location` int(11) NOT NULL,
  `segment` varchar(45) CHARACTER SET latin1 NOT NULL,
  `track` varchar(45) CHARACTER SET latin1 DEFAULT NULL,
  `coordinate` int(11) DEFAULT NULL,
  `hist` datetime DEFAULT NULL,
  PRIMARY KEY (`train_id`,`horario`,`location`,`segment`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainpmt`
--

DROP TABLE IF EXISTS `tbtrainpmt`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainpmt` (
  `pmt_id` varchar(25) CHARACTER SET latin1 NOT NULL,
  `prefix` varchar(5) CHARACTER SET latin1 NOT NULL,
  `prev_part` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `KMOrigem` smallint(6) NOT NULL DEFAULT '0',
  `KMDestino` smallint(6) NOT NULL DEFAULT '0',
  `sentflag` tinyint(1) NOT NULL DEFAULT '0',
  `OS` varchar(22) CHARACTER SET latin1 NOT NULL DEFAULT '',
  `valid` tinyint(1) DEFAULT '1',
  `insert_time` datetime DEFAULT NULL,
  `update_time` datetime DEFAULT NULL,
  `delete_time` datetime DEFAULT NULL,
  `date_hist` datetime NOT NULL,
  PRIMARY KEY (`pmt_id`,`prefix`),
  KEY `idxtrainpmt` (`prefix`,`date_hist`),
  KEY `idxos` (`OS`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainspeed`
--

DROP TABLE IF EXISTS `tbtrainspeed`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainspeed` (
  `train_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `name` varchar(5) CHARACTER SET latin1 NOT NULL,
  `data` datetime NOT NULL,
  `coordinate` int(11) NOT NULL,
  `speed` int(11) NOT NULL,
  `track` varchar(3) CHARACTER SET latin1 DEFAULT NULL,
  `datamili` int(11) NOT NULL,
  PRIMARY KEY (`train_id`),
  UNIQUE KEY `idxtrainspeed` (`name`,`data`,`coordinate`),
  KEY `idxtrainspeednome` (`name`),
  KEY `idxtrainspeednomedata` (`name`,`data`),
  KEY `idxtrainspeeddata` (`data`),
  KEY `idxtrainspeeddatamili` (`data`,`datamili`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainstatusqueue`
--

DROP TABLE IF EXISTS `tbtrainstatusqueue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainstatusqueue` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `train_id` bigint(20) NOT NULL,
  `horario` datetime NOT NULL,
  `status` varchar(15) CHARACTER SET latin1 NOT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idxtrainstatusqueue` (`hist`)
) ENGINE=InnoDB AUTO_INCREMENT=24 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtrainstopreason`
--

DROP TABLE IF EXISTS `tbtrainstopreason`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtrainstopreason` (
  `reason_id` smallint(6) NOT NULL,
  `descricao` varchar(30) CHARACTER SET latin1 NOT NULL,
  `isvisible` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`reason_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbtraintype`
--

DROP TABLE IF EXISTS `tbtraintype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbtraintype` (
  `prefix_id` varchar(20) CHARACTER SET latin1 NOT NULL,
  `name` varchar(20) CHARACTER SET latin1 NOT NULL,
  PRIMARY KEY (`prefix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbud`
--

DROP TABLE IF EXISTS `tbud`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbud` (
  `patio` smallint(6) NOT NULL,
  `ud` varchar(12) CHARACTER SET latin1 NOT NULL,
  `coordinate` int(11) NOT NULL,
  `direction` tinyint(4) NOT NULL,
  `isEnable` tinyint(1) NOT NULL DEFAULT '0',
  `nome` smallint(6) NOT NULL DEFAULT '0',
  PRIMARY KEY (`patio`,`ud`,`direction`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbunilogqueuein`
--

DROP TABLE IF EXISTS `tbunilogqueuein`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbunilogqueuein` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `msg` varchar(512) CHARACTER SET latin1 NOT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idxunilogqueuein` (`hist`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbunilogqueueout`
--

DROP TABLE IF EXISTS `tbunilogqueueout`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbunilogqueueout` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `msg` varchar(512) CHARACTER SET latin1 NOT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idxunilogqueueout` (`hist`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbunilogsendqueue`
--

DROP TABLE IF EXISTS `tbunilogsendqueue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbunilogsendqueue` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `msg` text CHARACTER SET latin1 NOT NULL,
  `hist` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbuser`
--

DROP TABLE IF EXISTS `tbuser`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbuser` (
  `id` int(11) NOT NULL,
  `senha` varchar(100) CHARACTER SET latin1 NOT NULL,
  `nome` varchar(30) CHARACTER SET latin1 NOT NULL,
  `email` varchar(60) CHARACTER SET latin1 DEFAULT NULL,
  `ramal` varchar(10) CHARACTER SET latin1 DEFAULT NULL,
  `motivo` varchar(255) CHARACTER SET latin1 DEFAULT NULL,
  `ga` varchar(6) CHARACTER SET latin1 DEFAULT NULL,
  `gg` varchar(6) CHARACTER SET latin1 DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbuseracess`
--

DROP TABLE IF EXISTS `tbuseracess`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbuseracess` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `data` datetime NOT NULL,
  `app_id` int(11) DEFAULT NULL,
  `user_id` int(11) NOT NULL,
  `ipaddr` varchar(15) CHARACTER SET latin1 DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `user_id` (`user_id`),
  KEY `app_id` (`app_id`),
  CONSTRAINT `tbuseracess_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `tbuser` (`id`),
  CONSTRAINT `tbuseracess_ibfk_2` FOREIGN KEY (`app_id`) REFERENCES `tbapp` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbuseradmapp`
--

DROP TABLE IF EXISTS `tbuseradmapp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbuseradmapp` (
  `user_id` int(11) NOT NULL,
  `app_id` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`user_id`,`app_id`),
  KEY `idxUserIdAdm` (`user_id`),
  KEY `idxAppIdAdm` (`app_id`),
  CONSTRAINT `tbuseradmapp_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `tbuser` (`id`),
  CONSTRAINT `tbuseradmapp_ibfk_2` FOREIGN KEY (`app_id`) REFERENCES `tbapp` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbuserapp`
--

DROP TABLE IF EXISTS `tbuserapp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbuserapp` (
  `user_id` int(11) NOT NULL,
  `app_id` int(11) NOT NULL,
  `auth_id` int(11) DEFAULT NULL,
  `isallowed` tinyint(1) DEFAULT NULL,
  `data` datetime DEFAULT NULL,
  PRIMARY KEY (`user_id`,`app_id`),
  KEY `idxUserIdApp` (`user_id`),
  KEY `idxAppIdApp` (`app_id`),
  KEY `idxAuthIdApp` (`auth_id`),
  CONSTRAINT `tbuserapp_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `tbuser` (`id`),
  CONSTRAINT `tbuserapp_ibfk_2` FOREIGN KEY (`app_id`) REFERENCES `tbapp` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbyards`
--

DROP TABLE IF EXISTS `tbyards`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tbyards` (
  `km_id` varchar(8) NOT NULL,
  `patio` varchar(6) DEFAULT NULL,
  `start_coordinate` int(11) NOT NULL DEFAULT '0',
  `end_coordinate` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`km_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Temporary view structure for view `vwpassageiro`
--

DROP TABLE IF EXISTS `vwpassageiro`;
/*!50001 DROP VIEW IF EXISTS `vwpassageiro`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE VIEW `vwpassageiro` AS SELECT 
 1 AS `train_id`,
 1 AS `train_name`,
 1 AS `direction`,
 1 AS `segment`,
 1 AS `previsao`,
 1 AS `hr_real`,
 1 AS `delta`*/;
SET character_set_client = @saved_cs_client;

--
-- Final view structure for view `vwpassageiro`
--

/*!50001 DROP VIEW IF EXISTS `vwpassageiro`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_general_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`eggo`@`%` SQL SECURITY DEFINER */
/*!50001 VIEW `vwpassageiro` AS select `tbtrain`.`train_id` AS `train_id`,`tbplan`.`train_name` AS `train_name`,`tbtrain`.`direction` AS `direction`,`tbplanpointspass`.`segment` AS `segment`,`tbplanpointspass`.`horario` AS `previsao`,`tbtrainmov`.`horario` AS `hr_real`,(select `tbpassageiro`.`delta_time_min` from `tbpassageiro` where ((`tbpassageiro`.`segment` = `tbplanpointspass`.`segment`) and (`tbpassageiro`.`direction` = `tbtrain`.`direction`))) AS `delta` from (((`tbplan` left join `tbplanpointspass` on((`tbplanpointspass`.`plan_id` = `tbplan`.`plan_id`))) left join `tbtrain` on((`tbplanpointspass`.`plan_id` = `tbtrain`.`plan_id`))) left join `tbtrainmov` on((`tbtrainmov`.`train_id` = `tbtrain`.`train_id`))) where ((`tbplanpointspass`.`plan_id` = (select `tbtrain`.`plan_id` from `tbtrain` where ((`tbtrain`.`name` like 'P02%') and (cast(`tbtrain`.`departure_time` as date) = cast(now() as date))) order by `tbtrain`.`departure_time` desc limit 1)) and (`tbplanpointspass`.`segment` in ('1','1_2','9','9_10','10','10_11','14','14_15','17','17_18','18','18_19','19','19_20','20','20_21','21','21_22','24','24_25','33','33_34','43','43_44','48','48_49','51','51_52','56','56_57'))) order by `tbplanpointspass`.`horario` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2018-01-10 22:56:09
