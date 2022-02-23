DROP TABLE IF EXISTS persona;
CREATE TABLE persona (id serial, 
					  home_sector text,
					  work_sector text,
					  gender text,
					  age int,
					  PRIMARY KEY (id));

SELECT AddGeometryColumn ('public','persona','home_location',4326,'POINT',2);
SELECT AddGeometryColumn ('public','persona','work_location',4326,'POINT',2);
CREATE INDEX "persona_gender_idx" ON "public"."persona" USING BTREE ("gender");
CREATE INDEX "persona_age_idx" ON "public"."persona" USING BTREE ("age");
