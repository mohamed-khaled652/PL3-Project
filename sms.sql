-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3308
-- Generation Time: Dec 13, 2024 at 06:59 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `sms`
--

-- --------------------------------------------------------

--
-- Table structure for table `instructor`
--

CREATE TABLE `instructor` (
  `AI` int(100) NOT NULL,
  `ID` int(100) NOT NULL,
  `name` varchar(200) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `instructor`
--

INSERT INTO `instructor` (`AI`, `ID`, `name`) VALUES
(6, 23456, 'shreen');

-- --------------------------------------------------------

--
-- Table structure for table `students`
--

CREATE TABLE `students` (
  `AI` int(100) NOT NULL,
  `ID` int(100) NOT NULL,
  `name` varchar(200) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `students`
--

INSERT INTO `students` (`AI`, `ID`, `name`) VALUES
(18, 20210770, 'mohamed khaled'),
(19, 20210772, 'mohamed khaled'),
(20, 20210773, 'mohamed khaled'),
(21, 20210860, 'mahmoud khaled'),
(23, 20210866, 'mahmoud adel');

-- --------------------------------------------------------

--
-- Table structure for table `students_details`
--

CREATE TABLE `students_details` (
  `AI` int(100) NOT NULL,
  `ID` int(100) NOT NULL,
  `course` varchar(200) NOT NULL,
  `grades` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `students_details`
--

INSERT INTO `students_details` (`AI`, `ID`, `course`, `grades`) VALUES
(39, 20210770, 'DSP', 85),
(40, 20210770, 'NN', 89),
(41, 20210770, 'Data Sience', 95),
(42, 20210770, 'ethics', 75),
(43, 20210770, 'pl3', 86),
(44, 20210770, 'Image Proccessing', 68),
(45, 20210772, 'Image Proccessing', 86),
(46, 20210772, 'DSP', 78),
(47, 20210772, 'ethics', 89),
(48, 20210772, 'pl3', 76),
(49, 20210772, 'NN', 97),
(50, 20210772, 'Data Science', 82),
(51, 20210773, 'Data Science', 78),
(52, 20210773, 'DSP', 35),
(53, 20210773, 'NN', 48),
(54, 20210773, 'ethics', 87),
(55, 20210773, 'Image Proccessing', 57),
(56, 20210773, 'pl3', 99),
(60, 20210860, 'pl3', 80),
(61, 20210860, 'DSP', 81),
(62, 20210860, 'Image Proccessing', 82),
(63, 20210860, 'NN', 83),
(64, 20210860, 'ethics', 84),
(65, 20210860, 'Data Science', 85),
(69, 20210866, 'pl3', 89);

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `ID` int(100) NOT NULL,
  `username` varchar(200) NOT NULL,
  `password` varchar(200) NOT NULL,
  `user_type` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`ID`, `username`, `password`, `user_type`) VALUES
(1, 'mohamed@gmail.com', '123', 'superuser'),
(23456, 'shreen@gmail.com', '123', 'instructor'),
(20210770, 'mex@gmail.com', '123', 'student'),
(20210772, 'king@gmail.com', '123', 'student'),
(20210773, 'tweety@gmail.com', '123', 'student'),
(20210860, 'te7a@gmail.com', '123', 'student'),
(20210866, 'adel@gmail.com', '123', 'student');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `instructor`
--
ALTER TABLE `instructor`
  ADD PRIMARY KEY (`AI`),
  ADD KEY `ID` (`ID`);

--
-- Indexes for table `students`
--
ALTER TABLE `students`
  ADD PRIMARY KEY (`AI`),
  ADD KEY `ID` (`ID`);

--
-- Indexes for table `students_details`
--
ALTER TABLE `students_details`
  ADD PRIMARY KEY (`AI`),
  ADD KEY `ID` (`ID`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`ID`),
  ADD UNIQUE KEY `username` (`username`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `instructor`
--
ALTER TABLE `instructor`
  MODIFY `AI` int(100) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `students`
--
ALTER TABLE `students`
  MODIFY `AI` int(100) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=24;

--
-- AUTO_INCREMENT for table `students_details`
--
ALTER TABLE `students_details`
  MODIFY `AI` int(100) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=70;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `instructor`
--
ALTER TABLE `instructor`
  ADD CONSTRAINT `instructor_ibfk_1` FOREIGN KEY (`ID`) REFERENCES `users` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `students`
--
ALTER TABLE `students`
  ADD CONSTRAINT `students_ibfk_1` FOREIGN KEY (`ID`) REFERENCES `users` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `students_details`
--
ALTER TABLE `students_details`
  ADD CONSTRAINT `students_details_ibfk_1` FOREIGN KEY (`ID`) REFERENCES `users` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
