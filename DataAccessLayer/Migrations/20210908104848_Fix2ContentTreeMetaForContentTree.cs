using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class Fix2ContentTreeMetaForContentTree : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"
-- Update CourseId for Biology
Update public.""ContentTree"" SET ""CourseId"" = '26CED180-9A6D-47A2-93D8-4E3731628D09'
WHERE ""NodeId"" IN(
	WITH RECURSIVE nodes(""NodeId"", ""CourseId"", ""ContentTreeMetaId"", ""ParentId"", ""Name"", ""Order"", ""IconId"") AS(
		SELECT CT1.""NodeId"", CT1.""CourseId"", CT1.""ContentTreeMetaId"", CT1.""ParentId"", CT1.""Name"", CT1.""Order"", CT1.""IconId""
		FROM public.""ContentTree"" as CT1 WHERE(CT1.""CourseId"" = '6553E323-828F-44E2-BFA1-38584CB056F0' AND CT1.""ContentTreeMetaId"" = '491FC325-F62E-4943-BD74-73EBAB0BFE1B' AND (CT1.""Name"" LIKE '%Unit 3%' OR CT1.""Name"" LIKE '%Unit 4%'))
			UNION
		SELECT CT2.""NodeId"", CT2.""CourseId"", CT2.""ContentTreeMetaId"", CT2.""ParentId"", CT2.""Name"", CT2.""Order"", CT2.""IconId""
		FROM public.""ContentTree"" as CT2, nodes CT1 WHERE CT2.""ParentId"" = CT1.""NodeId""
	)
	SELECT ""NodeId"" FROM nodes
);

-- Update ContentTreeMeta
-- AOS
Update public.""ContentTree"" SET ""ContentTreeMetaId"" = '247FCE3D-8AF3-4672-B313-A1186DA23679'
WHERE ""ContentTreeMetaId"" = '86A10A2B-3ACA-4940-A33D-4963307B8F3F' AND ""NodeId"" IN(
	WITH RECURSIVE nodes(""NodeId"", ""CourseId"", ""ContentTreeMetaId"", ""ParentId"", ""Name"", ""Order"", ""IconId"") AS(
		SELECT CT1.""NodeId"", CT1.""CourseId"", CT1.""ContentTreeMetaId"", CT1.""ParentId"", CT1.""Name"", CT1.""Order"", CT1.""IconId""
		FROM public.""ContentTree"" as CT1 WHERE(CT1.""CourseId"" = '26CED180-9A6D-47A2-93D8-4E3731628D09' AND CT1.""ContentTreeMetaId"" = '491FC325-F62E-4943-BD74-73EBAB0BFE1B' AND (CT1.""Name"" LIKE '%Unit 3%' OR CT1.""Name"" LIKE '%Unit 4%'))
			UNION
		SELECT CT2.""NodeId"", CT2.""CourseId"", CT2.""ContentTreeMetaId"", CT2.""ParentId"", CT2.""Name"", CT2.""Order"", CT2.""IconId""
		FROM public.""ContentTree"" as CT2, nodes CT1 WHERE CT2.""ParentId"" = CT1.""NodeId""
	)
	SELECT ""NodeId"" FROM nodes
);
-- Key Knoledge
Update public.""ContentTree"" SET ""ContentTreeMetaId"" = '3B1DBDE4-3ECD-455E-B16B-7DBC34EB7A68'
WHERE ""ContentTreeMetaId"" = 'AAC810BE-C2DD-4AF0-9D9B-D22613357904' AND ""NodeId"" IN(
	WITH RECURSIVE nodes(""NodeId"", ""CourseId"", ""ContentTreeMetaId"", ""ParentId"", ""Name"", ""Order"", ""IconId"") AS(
		SELECT CT1.""NodeId"", CT1.""CourseId"", CT1.""ContentTreeMetaId"", CT1.""ParentId"", CT1.""Name"", CT1.""Order"", CT1.""IconId""
		FROM public.""ContentTree"" as CT1 WHERE(CT1.""CourseId"" = '26CED180-9A6D-47A2-93D8-4E3731628D09' AND CT1.""ContentTreeMetaId"" = '491FC325-F62E-4943-BD74-73EBAB0BFE1B' AND (CT1.""Name"" LIKE '%Unit 3%' OR CT1.""Name"" LIKE '%Unit 4%'))
			UNION
		SELECT CT2.""NodeId"", CT2.""CourseId"", CT2.""ContentTreeMetaId"", CT2.""ParentId"", CT2.""Name"", CT2.""Order"", CT2.""IconId""
		FROM public.""ContentTree"" as CT2, nodes CT1 WHERE CT2.""ParentId"" = CT1.""NodeId""
	)
	SELECT ""NodeId"" FROM nodes
);
-- Topic
Update public.""ContentTree"" SET ""ContentTreeMetaId"" = '22268719-A01D-4D8B-A400-EB0AB36D182D'
WHERE ""ContentTreeMetaId"" = '80297E95-C6EC-4D2B-8E71-9C999A1CAFDA' AND ""NodeId"" IN(
	WITH RECURSIVE nodes(""NodeId"", ""CourseId"", ""ContentTreeMetaId"", ""ParentId"", ""Name"", ""Order"", ""IconId"") AS(
		SELECT CT1.""NodeId"", CT1.""CourseId"", CT1.""ContentTreeMetaId"", CT1.""ParentId"", CT1.""Name"", CT1.""Order"", CT1.""IconId""
		FROM public.""ContentTree"" as CT1 WHERE(CT1.""CourseId"" = '26CED180-9A6D-47A2-93D8-4E3731628D09' AND CT1.""ContentTreeMetaId"" = '491FC325-F62E-4943-BD74-73EBAB0BFE1B' AND (CT1.""Name"" LIKE '%Unit 3%' OR CT1.""Name"" LIKE '%Unit 4%'))
			UNION
		SELECT CT2.""NodeId"", CT2.""CourseId"", CT2.""ContentTreeMetaId"", CT2.""ParentId"", CT2.""Name"", CT2.""Order"", CT2.""IconId""
		FROM public.""ContentTree"" as CT2, nodes CT1 WHERE CT2.""ParentId"" = CT1.""NodeId""
	)
	SELECT ""NodeId"" FROM nodes
);
-- Level
Update public.""ContentTree"" SET ""ContentTreeMetaId"" = '8B274D5F-2965-4132-9806-822C8908D82C'
WHERE ""ContentTreeMetaId"" = '63FC8E2A-B9FD-49D7-A70C-CB8A23AD56BF' AND ""NodeId"" IN(
	WITH RECURSIVE nodes(""NodeId"", ""CourseId"", ""ContentTreeMetaId"", ""ParentId"", ""Name"", ""Order"", ""IconId"") AS(
		SELECT CT1.""NodeId"", CT1.""CourseId"", CT1.""ContentTreeMetaId"", CT1.""ParentId"", CT1.""Name"", CT1.""Order"", CT1.""IconId""
		FROM public.""ContentTree"" as CT1 WHERE(CT1.""CourseId"" = '26CED180-9A6D-47A2-93D8-4E3731628D09' AND CT1.""ContentTreeMetaId"" = '491FC325-F62E-4943-BD74-73EBAB0BFE1B' AND (CT1.""Name"" LIKE '%Unit 3%' OR CT1.""Name"" LIKE '%Unit 4%'))
			UNION
		SELECT CT2.""NodeId"", CT2.""CourseId"", CT2.""ContentTreeMetaId"", CT2.""ParentId"", CT2.""Name"", CT2.""Order"", CT2.""IconId""
		FROM public.""ContentTree"" as CT2, nodes CT1 WHERE CT2.""ParentId"" = CT1.""NodeId""
	)
	SELECT ""NodeId"" FROM nodes
);
-- Unit
Update public.""ContentTree"" SET ""ContentTreeMetaId"" = '9575FD04-0E50-4D08-BE4A-1F85F7EDA46F'
WHERE ""ContentTreeMetaId"" = '491FC325-F62E-4943-BD74-73EBAB0BFE1B' AND ""NodeId"" IN(
	WITH RECURSIVE nodes(""NodeId"", ""CourseId"", ""ContentTreeMetaId"", ""ParentId"", ""Name"", ""Order"", ""IconId"") AS(
		SELECT CT1.""NodeId"", CT1.""CourseId"", CT1.""ContentTreeMetaId"", CT1.""ParentId"", CT1.""Name"", CT1.""Order"", CT1.""IconId""
		FROM public.""ContentTree"" as CT1 WHERE(CT1.""CourseId"" = '26CED180-9A6D-47A2-93D8-4E3731628D09' AND CT1.""ContentTreeMetaId"" = '491FC325-F62E-4943-BD74-73EBAB0BFE1B' AND (CT1.""Name"" LIKE '%Unit 3%' OR CT1.""Name"" LIKE '%Unit 4%'))
			UNION
		SELECT CT2.""NodeId"", CT2.""CourseId"", CT2.""ContentTreeMetaId"", CT2.""ParentId"", CT2.""Name"", CT2.""Order"", CT2.""IconId""
		FROM public.""ContentTree"" as CT2, nodes CT1 WHERE CT2.""ParentId"" = CT1.""NodeId""
	)
	SELECT ""NodeId"" FROM nodes
);

-- Update CourseId for Chemistry
Update public.""ContentTree"" SET ""CourseId"" = 'DAF966AB-774E-4D52-A806-FF7241217A02'
WHERE ""NodeId"" IN(
	WITH RECURSIVE nodes(""NodeId"", ""CourseId"", ""ContentTreeMetaId"", ""ParentId"", ""Name"", ""Order"", ""IconId"") AS(
		SELECT CT1.""NodeId"", CT1.""CourseId"", CT1.""ContentTreeMetaId"", CT1.""ParentId"", CT1.""Name"", CT1.""Order"", CT1.""IconId""
		FROM public.""ContentTree"" as CT1 WHERE(CT1.""CourseId"" = 'A00E14AB-ADC8-40B4-AC03-6DF1FC9A9B46' AND CT1.""ContentTreeMetaId"" = 'AF4BDC3F-29C9-419D-80D1-69D681AC2059' AND (CT1.""Name"" LIKE '%Unit 3%' OR CT1.""Name"" LIKE '%Unit 4%'))
			UNION
		SELECT CT2.""NodeId"", CT2.""CourseId"", CT2.""ContentTreeMetaId"", CT2.""ParentId"", CT2.""Name"", CT2.""Order"", CT2.""IconId""
		FROM public.""ContentTree"" as CT2, nodes CT1 WHERE CT2.""ParentId"" = CT1.""NodeId""
	)
	SELECT ""NodeId"" FROM nodes
);

-- Update ContentTreeMeta
-- AOS
Update public.""ContentTree"" SET ""ContentTreeMetaId"" = '651205B5-A631-4E8E-8281-8C7280395250'
WHERE ""ContentTreeMetaId"" = '1A7C08CD-C47F-4D5E-8B3B-2EF11DC37730' AND ""NodeId"" IN(
	WITH RECURSIVE nodes(""NodeId"", ""CourseId"", ""ContentTreeMetaId"", ""ParentId"", ""Name"", ""Order"", ""IconId"") AS(
		SELECT CT1.""NodeId"", CT1.""CourseId"", CT1.""ContentTreeMetaId"", CT1.""ParentId"", CT1.""Name"", CT1.""Order"", CT1.""IconId""
		FROM public.""ContentTree"" as CT1 WHERE(CT1.""CourseId"" = 'DAF966AB-774E-4D52-A806-FF7241217A02' AND CT1.""ContentTreeMetaId"" = 'AF4BDC3F-29C9-419D-80D1-69D681AC2059' AND (CT1.""Name"" LIKE '%Unit 3%' OR CT1.""Name"" LIKE '%Unit 4%'))
			UNION
		SELECT CT2.""NodeId"", CT2.""CourseId"", CT2.""ContentTreeMetaId"", CT2.""ParentId"", CT2.""Name"", CT2.""Order"", CT2.""IconId""
		FROM public.""ContentTree"" as CT2, nodes CT1 WHERE CT2.""ParentId"" = CT1.""NodeId""
	)
	SELECT ""NodeId"" FROM nodes
);
-- Key Knoledge
Update public.""ContentTree"" SET ""ContentTreeMetaId"" = '61A75AED-0B99-4631-B402-9CD9F99DAB27'
WHERE ""ContentTreeMetaId"" = '22CEA254-804B-48E9-9E6F-DF36678A5A11' AND ""NodeId"" IN(
	WITH RECURSIVE nodes(""NodeId"", ""CourseId"", ""ContentTreeMetaId"", ""ParentId"", ""Name"", ""Order"", ""IconId"") AS(
		SELECT CT1.""NodeId"", CT1.""CourseId"", CT1.""ContentTreeMetaId"", CT1.""ParentId"", CT1.""Name"", CT1.""Order"", CT1.""IconId""
		FROM public.""ContentTree"" as CT1 WHERE(CT1.""CourseId"" = 'DAF966AB-774E-4D52-A806-FF7241217A02' AND CT1.""ContentTreeMetaId"" = 'AF4BDC3F-29C9-419D-80D1-69D681AC2059' AND (CT1.""Name"" LIKE '%Unit 3%' OR CT1.""Name"" LIKE '%Unit 4%'))
			UNION
		SELECT CT2.""NodeId"", CT2.""CourseId"", CT2.""ContentTreeMetaId"", CT2.""ParentId"", CT2.""Name"", CT2.""Order"", CT2.""IconId""
		FROM public.""ContentTree"" as CT2, nodes CT1 WHERE CT2.""ParentId"" = CT1.""NodeId""
	)
	SELECT ""NodeId"" FROM nodes
);
-- Topic
Update public.""ContentTree"" SET ""ContentTreeMetaId"" = 'A2CFB20C-2C96-4ADB-A953-E6A5B79419B6'
WHERE ""ContentTreeMetaId"" = 'E8753AA5-2939-48FD-AB9D-A186F211338F' AND ""NodeId"" IN(
	WITH RECURSIVE nodes(""NodeId"", ""CourseId"", ""ContentTreeMetaId"", ""ParentId"", ""Name"", ""Order"", ""IconId"") AS(
		SELECT CT1.""NodeId"", CT1.""CourseId"", CT1.""ContentTreeMetaId"", CT1.""ParentId"", CT1.""Name"", CT1.""Order"", CT1.""IconId""
		FROM public.""ContentTree"" as CT1 WHERE(CT1.""CourseId"" = 'DAF966AB-774E-4D52-A806-FF7241217A02' AND CT1.""ContentTreeMetaId"" = 'AF4BDC3F-29C9-419D-80D1-69D681AC2059' AND (CT1.""Name"" LIKE '%Unit 3%' OR CT1.""Name"" LIKE '%Unit 4%'))
			UNION
		SELECT CT2.""NodeId"", CT2.""CourseId"", CT2.""ContentTreeMetaId"", CT2.""ParentId"", CT2.""Name"", CT2.""Order"", CT2.""IconId""
		FROM public.""ContentTree"" as CT2, nodes CT1 WHERE CT2.""ParentId"" = CT1.""NodeId""
	)
	SELECT ""NodeId"" FROM nodes
);
-- Level
Update public.""ContentTree"" SET ""ContentTreeMetaId"" = 'FFED3946-9E2B-4B7D-906B-E1DDD41DFA36'
WHERE ""ContentTreeMetaId"" = 'F0CA0665-4DC1-4D79-BD45-1F5849E6FD40' AND ""NodeId"" IN(
	WITH RECURSIVE nodes(""NodeId"", ""CourseId"", ""ContentTreeMetaId"", ""ParentId"", ""Name"", ""Order"", ""IconId"") AS(
		SELECT CT1.""NodeId"", CT1.""CourseId"", CT1.""ContentTreeMetaId"", CT1.""ParentId"", CT1.""Name"", CT1.""Order"", CT1.""IconId""
		FROM public.""ContentTree"" as CT1 WHERE(CT1.""CourseId"" = 'DAF966AB-774E-4D52-A806-FF7241217A02' AND CT1.""ContentTreeMetaId"" = 'AF4BDC3F-29C9-419D-80D1-69D681AC2059' AND (CT1.""Name"" LIKE '%Unit 3%' OR CT1.""Name"" LIKE '%Unit 4%'))
			UNION
		SELECT CT2.""NodeId"", CT2.""CourseId"", CT2.""ContentTreeMetaId"", CT2.""ParentId"", CT2.""Name"", CT2.""Order"", CT2.""IconId""
		FROM public.""ContentTree"" as CT2, nodes CT1 WHERE CT2.""ParentId"" = CT1.""NodeId""
	)
	SELECT ""NodeId"" FROM nodes
);
-- Unit
Update public.""ContentTree"" SET ""ContentTreeMetaId"" = 'C67D52FB-FCFF-4F0B-BA46-9B782F4529C6'
WHERE ""ContentTreeMetaId"" = 'AF4BDC3F-29C9-419D-80D1-69D681AC2059' AND ""NodeId"" IN(
	WITH RECURSIVE nodes(""NodeId"", ""CourseId"", ""ContentTreeMetaId"", ""ParentId"", ""Name"", ""Order"", ""IconId"") AS(
		SELECT CT1.""NodeId"", CT1.""CourseId"", CT1.""ContentTreeMetaId"", CT1.""ParentId"", CT1.""Name"", CT1.""Order"", CT1.""IconId""
		FROM public.""ContentTree"" as CT1 WHERE(CT1.""CourseId"" = 'DAF966AB-774E-4D52-A806-FF7241217A02' AND CT1.""ContentTreeMetaId"" = 'AF4BDC3F-29C9-419D-80D1-69D681AC2059' AND (CT1.""Name"" LIKE '%Unit 3%' OR CT1.""Name"" LIKE '%Unit 4%'))
			UNION
		SELECT CT2.""NodeId"", CT2.""CourseId"", CT2.""ContentTreeMetaId"", CT2.""ParentId"", CT2.""Name"", CT2.""Order"", CT2.""IconId""
		FROM public.""ContentTree"" as CT2, nodes CT1 WHERE CT2.""ParentId"" = CT1.""NodeId""
	)
	SELECT ""NodeId"" FROM nodes
);

-- Update CourseId for Physics
Update public.""ContentTree"" SET ""CourseId"" = '3E82DA8E-5034-493D-A51B-87098BB35FBB'
WHERE ""NodeId"" IN(
	WITH RECURSIVE nodes(""NodeId"", ""CourseId"", ""ContentTreeMetaId"", ""ParentId"", ""Name"", ""Order"", ""IconId"") AS(
		SELECT CT1.""NodeId"", CT1.""CourseId"", CT1.""ContentTreeMetaId"", CT1.""ParentId"", CT1.""Name"", CT1.""Order"", CT1.""IconId""
		FROM public.""ContentTree"" as CT1 WHERE(CT1.""CourseId"" = '766EF33F-598F-4350-A691-2E27CA25B84D' AND CT1.""ContentTreeMetaId"" = 'C6CE6CAB-C2CD-4528-B465-2E4A1EEDFA45' AND (CT1.""Name"" LIKE '%Unit 3%' OR CT1.""Name"" LIKE '%Unit 4%'))
			UNION
		SELECT CT2.""NodeId"", CT2.""CourseId"", CT2.""ContentTreeMetaId"", CT2.""ParentId"", CT2.""Name"", CT2.""Order"", CT2.""IconId""
		FROM public.""ContentTree"" as CT2, nodes CT1 WHERE CT2.""ParentId"" = CT1.""NodeId""
	)
	SELECT ""NodeId"" FROM nodes
);

-- Update ContentTreeMeta
-- AOS
Update public.""ContentTree"" SET ""ContentTreeMetaId"" = 'EDE5B06F-A453-4679-A0BA-116F3011F697'
WHERE ""ContentTreeMetaId"" = 'FCABA401-0205-4DEF-A10C-1EC6404F236D' AND ""NodeId"" IN(
	WITH RECURSIVE nodes(""NodeId"", ""CourseId"", ""ContentTreeMetaId"", ""ParentId"", ""Name"", ""Order"", ""IconId"") AS(
		SELECT CT1.""NodeId"", CT1.""CourseId"", CT1.""ContentTreeMetaId"", CT1.""ParentId"", CT1.""Name"", CT1.""Order"", CT1.""IconId""
		FROM public.""ContentTree"" as CT1 WHERE(CT1.""CourseId"" = '3E82DA8E-5034-493D-A51B-87098BB35FBB' AND CT1.""ContentTreeMetaId"" = 'C6CE6CAB-C2CD-4528-B465-2E4A1EEDFA45' AND (CT1.""Name"" LIKE '%Unit 3%' OR CT1.""Name"" LIKE '%Unit 4%'))
			UNION
		SELECT CT2.""NodeId"", CT2.""CourseId"", CT2.""ContentTreeMetaId"", CT2.""ParentId"", CT2.""Name"", CT2.""Order"", CT2.""IconId""
		FROM public.""ContentTree"" as CT2, nodes CT1 WHERE CT2.""ParentId"" = CT1.""NodeId""
	)
	SELECT ""NodeId"" FROM nodes
);
-- Key Knoledge
Update public.""ContentTree"" SET ""ContentTreeMetaId"" = '805A5DF3-C1C2-426F-A84F-FA0A86238DED'
WHERE ""ContentTreeMetaId"" = '809BCA6E-1EB6-45BE-A4DC-1ED6621DFDB2' AND ""NodeId"" IN(
	WITH RECURSIVE nodes(""NodeId"", ""CourseId"", ""ContentTreeMetaId"", ""ParentId"", ""Name"", ""Order"", ""IconId"") AS(
		SELECT CT1.""NodeId"", CT1.""CourseId"", CT1.""ContentTreeMetaId"", CT1.""ParentId"", CT1.""Name"", CT1.""Order"", CT1.""IconId""
		FROM public.""ContentTree"" as CT1 WHERE(CT1.""CourseId"" = '3E82DA8E-5034-493D-A51B-87098BB35FBB' AND CT1.""ContentTreeMetaId"" = 'C6CE6CAB-C2CD-4528-B465-2E4A1EEDFA45' AND (CT1.""Name"" LIKE '%Unit 3%' OR CT1.""Name"" LIKE '%Unit 4%'))
			UNION
		SELECT CT2.""NodeId"", CT2.""CourseId"", CT2.""ContentTreeMetaId"", CT2.""ParentId"", CT2.""Name"", CT2.""Order"", CT2.""IconId""
		FROM public.""ContentTree"" as CT2, nodes CT1 WHERE CT2.""ParentId"" = CT1.""NodeId""
	)
	SELECT ""NodeId"" FROM nodes
);
-- Topic
Update public.""ContentTree"" SET ""ContentTreeMetaId"" = '8511CDAD-37F0-4D8C-AAC0-EBF6104660A4'
WHERE ""ContentTreeMetaId"" = 'D17244DF-AB06-4443-ABBA-FE1A14BFF3A9' AND ""NodeId"" IN(
	WITH RECURSIVE nodes(""NodeId"", ""CourseId"", ""ContentTreeMetaId"", ""ParentId"", ""Name"", ""Order"", ""IconId"") AS(
		SELECT CT1.""NodeId"", CT1.""CourseId"", CT1.""ContentTreeMetaId"", CT1.""ParentId"", CT1.""Name"", CT1.""Order"", CT1.""IconId""
		FROM public.""ContentTree"" as CT1 WHERE(CT1.""CourseId"" = '3E82DA8E-5034-493D-A51B-87098BB35FBB' AND CT1.""ContentTreeMetaId"" = 'C6CE6CAB-C2CD-4528-B465-2E4A1EEDFA45' AND (CT1.""Name"" LIKE '%Unit 3%' OR CT1.""Name"" LIKE '%Unit 4%'))
			UNION
		SELECT CT2.""NodeId"", CT2.""CourseId"", CT2.""ContentTreeMetaId"", CT2.""ParentId"", CT2.""Name"", CT2.""Order"", CT2.""IconId""
		FROM public.""ContentTree"" as CT2, nodes CT1 WHERE CT2.""ParentId"" = CT1.""NodeId""
	)
	SELECT ""NodeId"" FROM nodes
);
-- Level
Update public.""ContentTree"" SET ""ContentTreeMetaId"" = '75CE6881-ABBB-423E-B607-CDC80E4003FC'
WHERE ""ContentTreeMetaId"" = '22BC3EBC-8B73-41D5-B7CB-97B2D7858C9A' AND ""NodeId"" IN(
	WITH RECURSIVE nodes(""NodeId"", ""CourseId"", ""ContentTreeMetaId"", ""ParentId"", ""Name"", ""Order"", ""IconId"") AS(
		SELECT CT1.""NodeId"", CT1.""CourseId"", CT1.""ContentTreeMetaId"", CT1.""ParentId"", CT1.""Name"", CT1.""Order"", CT1.""IconId""
		FROM public.""ContentTree"" as CT1 WHERE(CT1.""CourseId"" = '3E82DA8E-5034-493D-A51B-87098BB35FBB' AND CT1.""ContentTreeMetaId"" = 'C6CE6CAB-C2CD-4528-B465-2E4A1EEDFA45' AND (CT1.""Name"" LIKE '%Unit 3%' OR CT1.""Name"" LIKE '%Unit 4%'))
			UNION
		SELECT CT2.""NodeId"", CT2.""CourseId"", CT2.""ContentTreeMetaId"", CT2.""ParentId"", CT2.""Name"", CT2.""Order"", CT2.""IconId""
		FROM public.""ContentTree"" as CT2, nodes CT1 WHERE CT2.""ParentId"" = CT1.""NodeId""
	)
	SELECT ""NodeId"" FROM nodes
);
-- Unit
Update public.""ContentTree"" SET ""ContentTreeMetaId"" = 'CDC844CD-9234-470A-BE57-B7E7C7F97A00'
WHERE ""ContentTreeMetaId"" = 'C6CE6CAB-C2CD-4528-B465-2E4A1EEDFA45' AND ""NodeId"" IN(
	WITH RECURSIVE nodes(""NodeId"", ""CourseId"", ""ContentTreeMetaId"", ""ParentId"", ""Name"", ""Order"", ""IconId"") AS(
		SELECT CT1.""NodeId"", CT1.""CourseId"", CT1.""ContentTreeMetaId"", CT1.""ParentId"", CT1.""Name"", CT1.""Order"", CT1.""IconId""
		FROM public.""ContentTree"" as CT1 WHERE(CT1.""CourseId"" = '3E82DA8E-5034-493D-A51B-87098BB35FBB' AND CT1.""ContentTreeMetaId"" = 'C6CE6CAB-C2CD-4528-B465-2E4A1EEDFA45' AND (CT1.""Name"" LIKE '%Unit 3%' OR CT1.""Name"" LIKE '%Unit 4%'))
			UNION
		SELECT CT2.""NodeId"", CT2.""CourseId"", CT2.""ContentTreeMetaId"", CT2.""ParentId"", CT2.""Name"", CT2.""Order"", CT2.""IconId""
		FROM public.""ContentTree"" as CT2, nodes CT1 WHERE CT2.""ParentId"" = CT1.""NodeId""
	)
	SELECT ""NodeId"" FROM nodes
);

");
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
