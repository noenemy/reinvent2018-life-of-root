/*
  - LifeOfRoot game용으로 새로 만들어봤습니다. 리뷰 필요합니다!!
     => 리뷰 완료 (jiskim@)
  - 모든 테이블/컬럼명은 소문자로 표기
  - end_date를 제외한 나머지 날짜 컬럼은 default 값으로 현재 시간 기록
*/

USE lifeofroot2018;

-- 카드 테스트용 테이블 2개 추가
-- 인식 테스트 결과 코드 테이블
CREATE TABLE tb_test_picture
(
  picture_id INT NOT NULL AUTO_INCREMENT,
  file_loc VARCHAR(2083) DEFAULT NULL, 
  use_yn VARCHAR(2) DEFAULT 'Y',
  PRIMARY KEY(picture_id)
) AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

-- 인식 테스트 결과 코드 테이블(상세)
CREATE TABLE tb_test_picture_labels
(
  picture_id INT NOT NULL,
  label_name VARCHAR(100),
  confidence DOUBLE,
  FOREIGN KEY(picture_id)
    REFERENCES tb_test_picture(picture_id)
    ON DELETE CASCADE
    ON UPDATE RESTRICT
)DEFAULT CHARSET=utf8;

CREATE TABLE tb_game
(
  game_id INT NOT NULL AUTO_INCREMENT,
  name VARCHAR(100),
  start_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, 
  end_date TIMESTAMP,
  PRIMARY KEY(game_id)
) AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

CREATE TABLE tb_stage_log
(
  game_id INT NOT NULL,
  stage_id INT NOT NULL,
  objects_score INT,
  time_score INT,
  clear_score INT,
  stage_score INT,
  completed_yn CHAR(1) NOT NULL,
  start_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  end_date TIMESTAMP,
  PRIMARY KEY(game_id,stage_id),
  FOREIGN KEY(game_id) 
    REFERENCES tb_game(game_id)
    ON DELETE CASCADE
    ON UPDATE RESTRICT
) DEFAULT CHARSET=utf8;

CREATE TABLE tb_stage_object
(
  game_id INT NOT NULL,
  stage_id INT NOT NULL,
  object_name VARCHAR(100) NOT NULL,
  object_score INT,
  found_yn CHAR(1) NOT NULL,
  file_loc VARCHAR(2083),
  log_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY(game_id,stage_id,object_name),
  FOREIGN KEY(game_id) 
    REFERENCES tb_game(game_id)
    ON DELETE CASCADE
    ON UPDATE RESTRICT
) DEFAULT CHARSET=utf8;

CREATE TABLE tb_object
(
  object_id INT NOT NULL,
  object_name VARCHAR(100) NOT NULL,
  object_score INT,
  difficulty INT,
  log_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY(object_id)
) DEFAULT CHARSET=utf8;

CREATE TABLE tb_game_result
(
  game_id INT NOT NULL,
  result_page_url VARCHAR(2083),
  total_score INT,
  total_rank BIGINT,
  total_found_objects INT,
  total_playtime INT,
  PRIMARY KEY(game_id),
  FOREIGN KEY(game_id) 
    REFERENCES tb_game(game_id)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT
) DEFAULT CHARSET=utf8;


=====================================

/*
  - 모든 테이블/컬럼명은 소문자로 표기
  - end_date를 제외한 나머지 날짜 컬럼은 default 값으로 현재 시간 기록
  - FK의 부모 키 삭제 시 FK 값도 삭제 / 변경 시 무시(...) -> 정해주세요
  - 랭킹 숫자값은 유저가 넘쳐나길 기원하는 마음으로 BIGINT로 설정
  - tb_game_rank_type 테이블의 'rank' 컬럼명은 예약어인 관계로 type_rank로 변경
  - URL 컬럼 타입은 스택오버플로의 중론에 따라 VARCHAR(2083)으로 지정 (MySQL 5.0 미만은 TEXT로, 상위 버전은 저걸로 하라고 되어 있음;;)
*/
use gottalent2018;

-- 게임 테이블. 유저 당 하나의 고유한 row가 생성되며, 플레이 완료 후 end_date가 업데이트되는 구조임
CREATE TABLE tb_game
(
  game_id INT NOT NULL AUTO_INCREMENT,
  name VARCHAR(100),
  share_yn CHAR(1) NOT NULL,
  start_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, 
  end_date TIMESTAMP,
  PRIMARY KEY(game_id)
) AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

-- 캐스팅 마스터 코드 테이블. 영화명, 배우(역할)명, 성별, 등급 정보, 이미지(S3), 연기유형(Emotion)
CREATE TABLE tb_cast 
(
  cast_id int(11) NOT NULL AUTO_INCREMENT,
  title varchar(200) DEFAULT NULL,
  actor varchar(100) DEFAULT NULL,
  gender varchar(10) DEFAULT NULL,
  grade varchar(2) DEFAULT NULL,
  file_loc varchar(2083) DEFAULT NULL,
  action_type varchar(100) DEFAULT NULL,
  PRIMARY KEY (`cast_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- 게임 내 스테이지 별 로그 테이블
CREATE TABLE tb_stage_log
(
  game_id INT NOT NULL,
  action_type VARCHAR(100) NOT NULL,
  score DOUBLE,
  file_loc VARCHAR(2083),
  age TINYINT,
  gender VARCHAR(10),
  log_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY(game_id,action_type),
  FOREIGN KEY(game_id) 
    REFERENCES tb_game(game_id)
    ON DELETE CASCADE
    ON UPDATE RESTRICT
) DEFAULT CHARSET=utf8;

-- 게임 별 최종 결과 테이블. 전체 스코어, 랭킹, 캐스팅 결과 저장. 랭킹(total_rank)컬럼만 전체 사용자 플레이 결과에 따라 업데이트됨
CREATE TABLE tb_game_result
(
  game_id INT NOT NULL,
  name VARCHAR(100),
  total_score INT,
  total_rank BIGINT,
  total_found_objects INT,
  total_playtime INT,
  PRIMARY KEY(game_id),
  FOREIGN KEY(game_id) 
    REFERENCES tb_game(game_id)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT
) DEFAULT CHARSET=utf8;

-- 스테이지 타입 별 랭킹 테이블. action_type 별로 랭킹을 산정/결과 
CREATE TABLE tb_game_rank_type
(
  game_id INT NOT NULL,
  action_type VARCHAR(100) NOT NULL,
  type_rank BIGINT,
  PRIMARY KEY(game_id,action_type),
  FOREIGN KEY(game_id) 
    REFERENCES tb_game(game_id)
    ON DELETE CASCADE
    ON UPDATE RESTRICT
) DEFAULT CHARSET=utf8;

-- 게임 내 NPC 피드백 대사 (총 82개)
CREATE TABLE tb_feedback
(
  feedback_id INT NOT NULL AUTO_INCREMENT,
  judge_gender VARCHAR(10) DEFAULT NULL, 
  action_type VARCHAR(100) NOT NULL,  
  grade varchar(10) DEFAULT NULL, 
  feedback VARCHAR(2000) DEFAULT NULL,
  PRIMARY KEY(feedback_id)
) DEFAULT CHARSET=utf8;



