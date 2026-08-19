--
-- PostgreSQL database dump
--

\restrict H0TrnMfxzvtevbNfFOy40q5XpOAY8EhhLImcGyDvaw1WiOEs9XR0roGyrS5SQIk

-- Dumped from database version 17.6
-- Dumped by pg_dump version 17.10

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET search_path = public;
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Data for Name: empresas; Type: TABLE DATA; Schema: public; Owner: -
--

SET SESSION AUTHORIZATION DEFAULT;

ALTER TABLE public.empresas DISABLE TRIGGER ALL;

COPY public.empresas (id, nombre_comercial, razon_social, identificacion_fiscal, logo_url, color_primario, color_secundario, zona_horaria, activa, created_at, updated_at, created_by, updated_by, deleted_at, deleted_by) FROM stdin;
1a3ef008-e768-43f3-9bbd-0be675710cf6	Inmoveg Perú	Inmoveg Perú S.A.C.	20000000000	\N	\N	\N	America/Lima	t	2026-07-17 17:56:59.60015+00	2026-07-17 17:56:59.60015+00	\N	\N	\N	\N
\.


ALTER TABLE public.empresas ENABLE TRIGGER ALL;

--
-- Data for Name: sucursales; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.sucursales DISABLE TRIGGER ALL;

COPY public.sucursales (id, empresa_id, nombre, descripcion, direccion, responsable_id, activa, created_at, updated_at, created_by, updated_by, deleted_at, deleted_by, codigo) FROM stdin;
1330cb6d-2785-488e-8710-adcb6d2fceb7	1a3ef008-e768-43f3-9bbd-0be675710cf6	Sede Principal	\N	Lima, Perú	\N	t	2026-07-17 18:02:22.81359+00	2026-07-17 18:02:22.81359+00	\N	\N	\N	\N	\N
c51e93ca-fe48-4e4d-9156-ad0b7cf14adf	1a3ef008-e768-43f3-9bbd-0be675710cf6	GVD CENTRO	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000000
c0a66b6a-1e05-4626-a454-8f74ba9c2d91	1a3ef008-e768-43f3-9bbd-0be675710cf6	GVD SUR	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000001
413bfabb-e9ca-4e4c-a8ee-29da97ec5e48	1a3ef008-e768-43f3-9bbd-0be675710cf6	GVD TRAPICHE	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000002
defbc875-9a7c-4ca9-98a8-7bfc539d9741	1a3ef008-e768-43f3-9bbd-0be675710cf6	GVD ESTE	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000003
429ee8ea-c848-4899-8075-d9e839dcb1c7	1a3ef008-e768-43f3-9bbd-0be675710cf6	GVD HUAURA	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000004
d7459f2e-2bdc-4d31-8f7d-ca37f3732706	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV AÑO NUEVO	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000005
4fe81c7a-bfb0-4895-aada-ca933b378a10	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV TRES REGIONES	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000006
b154c9f0-6318-41b8-aa7d-f2d85475744a	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV HUAMANTANGA	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000007
485d1d3c-9f07-495b-94a2-c174a6ee17a6	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV GAMBETA	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000008
991c58e7-2f60-43c4-8b48-e16bffbc9ba3	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV SAN ANTONIO	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000009
80fe494d-f8e4-4a3d-92e2-135518d73b4a	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV SAN DIEGO	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000010
b4cc744b-2cf8-49c9-a4a7-7e712eda3524	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV NARANJAL	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000011
fc5412e6-5d61-46e2-9e7f-e1e55a5809ed	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV COLLIQUE	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000012
d9360766-4640-4306-b657-cc6e75ceed27	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV CHORRILLOS CASH	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000013
b3ccfa59-9539-469e-99d5-b3fc983089a0	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV SURCO CASH	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000014
6aa05dbd-ea24-4c66-8559-8093b819d21f	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV INFANTAS	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000015
0634bd12-e076-4cca-b242-744dbd6d7498	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV MINKA CASH	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000016
28508ee3-bd89-4c16-b661-8e8cfe079615	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV COLONIAL	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000017
ed763381-88f8-4dba-8a3e-50bc6da287f2	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV FILOMENO	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000018
232cdbb0-7254-4c3f-b7ce-2531fafb706e	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV SANTA CLARA	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000019
a309decb-b134-41c7-a75d-5cf2614af839	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV VILLA CASH	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000020
dd96b794-2f3e-49de-b8c8-baeeebf10110	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV HUANDOY	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000021
07c96055-9c8d-4aa0-8fe6-84cb70c532d2	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV ALISOS	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000022
73ba3f62-6d53-4fde-9835-89a5a9eb7a61	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV RIOBAMBA	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000023
a7ca220c-fef2-47c5-8d54-a4caf5c63fd9	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV VARA DE ORO	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000024
c0c04b39-576b-406f-8e40-9ddb46d72b3b	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV MONTENEGRO	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000025
937a3eb1-44a5-4698-9d45-5bbf84f444f1	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV CHIMU	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000026
a48d3ab2-9057-45a4-980b-ce28ef27f028	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV AMANCAES 3	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000027
8ee3b1bb-b19f-4d41-89a1-eb93112f3c84	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV MARANGA	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000028
75dd30d6-3065-49b0-8b9a-caed469800a7	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV ESCARDO	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000029
8bc63ede-cea8-4b90-909f-04ce97e4a5ce	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV LAS GUINDAS	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000030
525cf24f-25d9-4130-a99b-b4328642b216	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV UNIVERSAL	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000031
406edb2b-4582-41e9-a9e9-2adf9e88080e	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV MARIANO PASTOR	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000032
552a018c-acd8-400c-a843-8c54006cb2d8	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV MARIANO CORNEJO	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000033
273a4b2c-0b9e-434b-9728-028622f9357d	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV CLEMENT	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000034
29c49932-e571-459d-a5c6-8236fb117053	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV INDEPENDENCIA	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000035
c07c9129-0597-4b51-a6dc-61b8b0f5a3fa	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV HUSARES DE JUNIN	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000036
a48bcd1f-39ca-4071-9c54-7a5f412c3d1a	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV AVIACION	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000037
ad770d46-b276-4f10-94a6-3cfe6ff5c73e	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV CANEVARO	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000038
6fb76529-a303-4bb4-93bb-2ba3e18b7dcf	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV LA CULTURA	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000039
eb8861dd-5449-4eee-a2ce-ebe8212f178a	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV ALAYZA	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000040
ea230cb4-65c2-45f0-9e75-2783595a1527	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV HIGUERETA	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000041
58481d89-8c10-4577-8122-2e0d3745173c	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV BENAVIDES	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000042
09740fe1-32b3-4a0a-bba9-229af07d980d	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV ROSPIGLIOSI	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000043
9eb4d41b-44df-43d5-b16e-b11f7442c14e	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV ROOSEVELT	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000044
b8ea0f2e-6393-4aa1-9544-dbef9f01a298	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV IGNACIO MERINO	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000045
35ff0902-1285-4d9b-970f-164efd7a23f0	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV LOS CEDROS	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000046
39b87ab6-4cca-4070-ab6e-8e26235ae8fd	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV SANTA CATALINA	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000047
024bd3f5-2753-469a-8d6e-b8ce3701afbe	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV SAN LUIS	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000048
81b5df23-1c62-4f5f-8366-0d65be3114ab	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV MARIATEGUI	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000049
56e97f2d-9cee-4b18-9c85-c4fb3e0fc995	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV MALVINAS	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000050
97281f0a-b6ac-4443-9809-c5f9cffb9dd1	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV LORETO	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000051
8ef96537-461a-4ea1-9571-10b68ae5362c	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV SALAMANCA	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000052
6bdae4a5-34eb-4821-abb3-99555b2939ee	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV LOS CONDORES	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000053
1670d277-ee51-484b-9b5c-3bdd93a55921	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV BOCANEGRA	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000054
0d2ae6f5-d405-4b11-b022-45a9527d3cc5	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV BELLAVISTA GRAU	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000055
bf23a7ab-aa01-4a57-8f0f-0db6d15248a6	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV AMARANTO	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000056
6dbc0989-6242-4ed2-9a26-5a567d808876	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV SANTO DOMINGO	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000057
ec9549bf-4a9e-4737-b1cc-3fa3fa064258	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV MALL COMAS	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000058
89201bd7-9aee-436c-b6a2-134eb1e99141	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV IZAGUIRRE	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000059
f449d839-5462-428f-8fe0-1febe69f9f7a	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV CANTA CALLAO	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000061
b1f49fdd-74f6-48fc-8379-fad3c727d669	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV LOS OLIVOS	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000062
ae24084b-b0c4-4820-826c-10c2646ddf75	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV JUPITER	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000063
a02a7935-c51a-4e95-b6de-4d14ccf89dee	1a3ef008-e768-43f3-9bbd-0be675710cf6	TALLER 2	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000064
7c4bc13b-5692-487e-aab5-c0b4454baf9a	1a3ef008-e768-43f3-9bbd-0be675710cf6	ENVASADORA TRAPICHE	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000065
b7747913-5b28-41d7-9380-ff50c6503bcd	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV VILLARAN	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000066
a5d560e9-12de-49b0-90c3-6ff283e7fc4c	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV MICAELA	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000067
998cd3df-eb18-458a-b8aa-cd3dffd64b18	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV LURIN	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000068
9b338def-265f-47c0-ae6c-b1e5e4d2b610	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV TANTAMAYO	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000069
7586e59f-567b-47e3-9f17-8c051f5ca09f	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV CHOSICA	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-24 15:51:27.562052+00	\N	\N	\N	\N	4000070
49030017-59bf-4ed0-9fa5-5c19aa2a96f1	1a3ef008-e768-43f3-9bbd-0be675710cf6	CV PALMERAS	\N	\N	\N	t	2026-07-24 15:51:27.562052+00	2026-07-27 14:59:06.293817+00	\N	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N	4000060
ca9c631f-d4f7-4f4f-a4c8-0b3cd60fb4e3	1a3ef008-e768-43f3-9bbd-0be675710cf6	TALLER 1	\N	\N	\N	t	2026-08-01 01:14:56.213189+00	2026-08-01 01:14:56.213189+00	\N	\N	\N	\N	4000071
\.


ALTER TABLE public.sucursales ENABLE TRIGGER ALL;

--
-- Data for Name: areas; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.areas DISABLE TRIGGER ALL;

COPY public.areas (id, sucursal_id, nombre, descripcion, responsable_id, activa, created_at, updated_at, created_by, updated_by, deleted_at, deleted_by) FROM stdin;
a404b2cb-7f5d-4d9e-9b38-1f9e46583755	1330cb6d-2785-488e-8710-adcb6d2fceb7	Recepción	\N	\N	t	2026-07-17 19:03:50.244716+00	2026-07-17 19:03:50.244716+00	21540103-61cc-4141-a3f1-11763957b648	\N	\N	\N
eb390d96-3a23-4e61-80da-de3d0b7552e1	1330cb6d-2785-488e-8710-adcb6d2fceb7	Locales alquilados	\N	\N	t	2026-07-17 20:08:36.371119+00	2026-07-17 20:08:36.371119+00	21540103-61cc-4141-a3f1-11763957b648	\N	\N	\N
2ac21108-ff6b-4d9d-aaeb-be26a32235d3	1330cb6d-2785-488e-8710-adcb6d2fceb7	prueba de error de ticket	\N	\N	t	2026-07-17 21:10:21.796247+00	2026-07-17 21:10:21.796247+00	02419c75-3006-4f51-8019-a435201f52ba	\N	\N	\N
7d36b302-971b-4358-bb1c-fd902b9d0a9d	1330cb6d-2785-488e-8710-adcb6d2fceb7	prueba	\N	\N	t	2026-07-18 18:14:23.555686+00	2026-07-18 18:14:23.555686+00	21540103-61cc-4141-a3f1-11763957b648	\N	\N	\N
f4897940-e51a-444d-b636-8f0143095e5b	1330cb6d-2785-488e-8710-adcb6d2fceb7	nueva area	\N	\N	t	2026-07-20 20:32:40.831058+00	2026-07-20 20:32:40.831058+00	21540103-61cc-4141-a3f1-11763957b648	\N	\N	\N
d3389d7e-2a3c-4c4f-8981-207fa57e8622	07c96055-9c8d-4aa0-8fe6-84cb70c532d2	nueva area	\N	\N	t	2026-07-31 01:33:16.752802+00	2026-07-31 01:33:16.752802+00	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	\N	\N	\N
030704f2-ec6b-4ca8-953f-e0985a7b2d6c	1330cb6d-2785-488e-8710-adcb6d2fceb7	Sistemas	\N	\N	t	2026-08-03 14:36:27.088811+00	2026-08-03 14:36:27.088811+00	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N	\N
d746db75-695c-4ddc-aa3c-d554b9c0047f	d7459f2e-2bdc-4d31-8f7d-ca37f3732706	Sistemas	\N	\N	t	2026-08-03 16:15:59.401113+00	2026-08-03 16:15:59.401113+00	25541c55-aafe-4714-98d1-a177b057302e	\N	\N	\N
310845d3-3780-4fff-ba04-c862e5fefaee	1330cb6d-2785-488e-8710-adcb6d2fceb7	Sistemas1	\N	\N	t	2026-08-04 16:04:26.586657+00	2026-08-04 16:04:26.586657+00	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N	\N
49e16dd4-ac01-42eb-beb8-46b84e40a987	a48d3ab2-9057-45a4-980b-ce28ef27f028	Sistemas2	\N	\N	t	2026-08-18 17:42:55.248664+00	2026-08-18 17:42:55.248664+00	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N	\N
\.


ALTER TABLE public.areas ENABLE TRIGGER ALL;

--
-- Data for Name: usuarios; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.usuarios DISABLE TRIGGER ALL;

COPY public.usuarios (id, auth_id, empresa_id, sucursal_id, area_id, nombre, apellido, correo, nombre_usuario, telefono, rol, estado_laboral, activo, foto_url, ultimo_acceso, version, created_at, updated_at, created_by, updated_by, deleted_at, deleted_by) FROM stdin;
21540103-61cc-4141-a3f1-11763957b648	65d34310-8b54-4aa3-b1b3-d690636d963b	1a3ef008-e768-43f3-9bbd-0be675710cf6	1330cb6d-2785-488e-8710-adcb6d2fceb7	\N	Raul	Trigoso	rtrigosovasquez@gmail.com	rtrigosovasquez	\N	SUPERADMIN	ACTIVO	t	\N	\N	3	2026-07-14 22:09:58.431767+00	2026-07-17 18:03:32.92721+00	\N	\N	\N	\N
02419c75-3006-4f51-8019-a435201f52ba	30eec1e0-d682-468a-8205-b12de4b59b1c	1a3ef008-e768-43f3-9bbd-0be675710cf6	1330cb6d-2785-488e-8710-adcb6d2fceb7	\N	Luis	Malito	luismalito@gmail.com	luismalito	\N	USUARIO	ACTIVO	t	\N	\N	1	2026-07-17 20:41:29.983318+00	2026-07-17 20:41:29.983318+00	21540103-61cc-4141-a3f1-11763957b648	\N	\N	\N
ffced565-d714-42ce-a4f1-995c9511441c	630fc7fb-16b7-4435-a578-50eeda7a5ad9	1a3ef008-e768-43f3-9bbd-0be675710cf6	1330cb6d-2785-488e-8710-adcb6d2fceb7	\N	Pruebas	Tecnicas	pruebastecnicas2026@gmail.com	pruebastecnicas2026	\N	ADMIN	ACTIVO	t	\N	\N	3	2026-07-21 14:15:21.996421+00	2026-07-27 20:07:18.346804+00	21540103-61cc-4141-a3f1-11763957b648	21540103-61cc-4141-a3f1-11763957b648	\N	\N
7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	1b6dc6e9-83a2-493c-9109-898b0b22c4fe	1a3ef008-e768-43f3-9bbd-0be675710cf6	1330cb6d-2785-488e-8710-adcb6d2fceb7	\N	Geancarlos	Barrionuevo	geancarlosbarrionuevo@outlook.com	geancarlosbarrionuevo	\N	TRABAJADOR	ACTIVO	t	\N	\N	7	2026-07-17 20:41:56.30643+00	2026-07-27 20:09:52.251567+00	21540103-61cc-4141-a3f1-11763957b648	21540103-61cc-4141-a3f1-11763957b648	\N	\N
25541c55-aafe-4714-98d1-a177b057302e	c28c00ac-2b5b-4589-80ea-349ac1f87d8f	1a3ef008-e768-43f3-9bbd-0be675710cf6	d7459f2e-2bdc-4d31-8f7d-ca37f3732706	\N	Claudia Natalia	Romero Andrade	rclaudianatalia@gmail.com	rclaudianatalia	+51973987140	USUARIO	ACTIVO	t	\N	\N	1	2026-08-03 15:57:40.959062+00	2026-08-03 15:57:40.959062+00	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N	\N
a72dcddd-405e-4d93-a819-d6f16bfc5f1c	9d539a76-2a65-4f23-bcba-f9a106b5d556	1a3ef008-e768-43f3-9bbd-0be675710cf6	d7459f2e-2bdc-4d31-8f7d-ca37f3732706	\N	Natalia	Romero	claromeroa@uch.pe	claromeroa	+51973987140	TRABAJADOR	ACTIVO	t	\N	\N	4	2026-08-04 17:01:47.989339+00	2026-08-18 17:51:04.509155+00	ffced565-d714-42ce-a4f1-995c9511441c	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
\.


ALTER TABLE public.usuarios ENABLE TRIGGER ALL;

--
-- Data for Name: audit_logs; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.audit_logs DISABLE TRIGGER ALL;

COPY public.audit_logs (id, actor_id, actor_nombre, actor_rol, accion, modulo, entidad_tipo, entidad_id, entidad_codigo, valor_anterior, valor_nuevo, ip, user_agent, sucursal_id, created_at) FROM stdin;
818ec393-545b-4e60-b2ba-ba6decb720bc	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREADO	sistema	tipos_servicio	08279f43-36da-491f-bb06-256f388add5a	\N	\N	{"Orden": 1, "Nombre": "Soporte de Hardware", "EmpresaId": "1a3ef008-e768-43f3-9bbd-0be675710cf6"}	\N	\N	\N	2026-07-17 18:56:06.540567+00
563da4fa-0560-49cf-8754-f1b2ea4d5a9d	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREADO	sistema	tipos_servicio	e6978a34-0c76-4169-a82c-9348bb6ba9b5	\N	\N	{"Orden": 2, "Nombre": "Soporte de Software", "EmpresaId": "1a3ef008-e768-43f3-9bbd-0be675710cf6"}	\N	\N	\N	2026-07-17 18:56:27.561024+00
c6bac89d-82a9-4605-9d54-8e1c11d921d7	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREADO	sistema	tipos_servicio	957a3ea8-f00b-4006-b530-7ab456c9dac0	\N	\N	{"Orden": 3, "Nombre": "Infraestructura y Redes", "EmpresaId": "1a3ef008-e768-43f3-9bbd-0be675710cf6"}	\N	\N	\N	2026-07-17 18:56:38.59138+00
09736c44-cbc1-4c85-b723-357222801ad8	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREADO	sistema	tipos_servicio	641ca311-3280-4faa-ac49-285822a16f02	\N	\N	{"Orden": 4, "Nombre": "Mantenimiento de Instalaciones", "EmpresaId": "1a3ef008-e768-43f3-9bbd-0be675710cf6"}	\N	\N	\N	2026-07-17 18:56:49.747902+00
9eaf650f-ae27-4343-a5cd-b511102e7a39	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREADO	sistema	tipos_servicio	d05fdbc7-a2d0-4a1f-b5f8-aac4f1f1227e	\N	\N	{"Orden": 5, "Nombre": "Solicitud de Servicios Generales", "EmpresaId": "1a3ef008-e768-43f3-9bbd-0be675710cf6"}	\N	\N	\N	2026-07-17 18:56:58.280242+00
f1e66ba0-cfee-419b-9146-d0d1ad1ca07b	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREADO	sistema	categorias	fc80bf18-53f6-4a9b-b486-2dae9861c2c8	\N	\N	{"Nombre": "Tecnología (TI)", "EsGlobal": true, "EmpresaId": null}	\N	\N	\N	2026-07-17 18:57:08.87942+00
5cabd3a3-b683-497f-9aab-541d6d6cef55	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREADO	sistema	categorias	10c2eeb5-e1b3-4d28-9efb-7eb07be2b8e8	\N	\N	{"Nombre": "Infraestructura", "EsGlobal": true, "EmpresaId": null}	\N	\N	\N	2026-07-17 18:57:18.137675+00
058e3897-6b0e-4ad8-bdb4-93947045eb6d	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREADO	sistema	categorias	2e162bf3-1818-4608-8c4c-d199b46f68ba	\N	\N	{"Nombre": "Administración", "EsGlobal": true, "EmpresaId": null}	\N	\N	\N	2026-07-17 18:57:26.452218+00
cd0a7c30-be19-4761-b34c-36c69e6fa4b9	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREADO	sistema	categorias	c4cb8565-fe4f-4222-a4b9-97e74ee70edd	\N	\N	{"Nombre": "Operaciones", "EsGlobal": true, "EmpresaId": null}	\N	\N	\N	2026-07-17 18:57:34.773383+00
bd20160e-e1f8-4f23-b853-66a644b840aa	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREADO	sistema	categorias	1b567db9-9f99-4c67-86ab-4885deb216dc	\N	\N	{"Nombre": "Servicios Generales", "EsGlobal": true, "EmpresaId": null}	\N	\N	\N	2026-07-17 18:57:42.73761+00
e8309bd6-fd2f-4056-8f21-3d839d669b5f	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	tickets	6d09f602-356f-457c-ba23-e65000d73c22	\N	\N	{"Estado": 1, "Titulo": "Foco quemado en la recepción principal", "SolicitanteId": "21540103-61cc-4141-a3f1-11763957b648"}	\N	\N	\N	2026-07-17 19:03:50.654051+00
62fff5fa-d062-4521-a211-befb22cf21c4	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	DESACTIVADO	sistema	tipos_servicio	08279f43-36da-491f-bb06-256f388add5a	\N	{"Activo": true}	{"Activo": false}	\N	\N	\N	2026-07-17 19:43:19.789228+00
4e0199e7-40c4-4e98-8102-4b3cde331d85	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREADO	sistema	tipos_servicio	1985f3ba-365f-4d6f-b601-137fc846f0f6	\N	\N	{"Orden": 6, "Nombre": "Electrico", "EmpresaId": "1a3ef008-e768-43f3-9bbd-0be675710cf6"}	\N	\N	\N	2026-07-17 19:50:28.954573+00
5e894a5d-166e-4022-a266-1607234ecd8e	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	tickets	496824d7-63ad-427b-b648-3357ccd20b83	\N	\N	{"Estado": 1, "Titulo": "error de tableros", "SolicitanteId": "21540103-61cc-4141-a3f1-11763957b648"}	\N	\N	\N	2026-07-17 20:08:36.837056+00
0fc105e1-afe8-485b-b06b-5bc4c1f1bf37	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CAMBIAR_PRIORIDAD	sistema	tickets	496824d7-63ad-427b-b648-3357ccd20b83	\N	{"Prioridad": "ALTA"}	{"Prioridad": "MEDIA"}	\N	\N	\N	2026-07-17 20:13:07.762482+00
c41b7db2-4461-4608-adc4-7c66c32341d2	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CAMBIAR_PRIORIDAD	sistema	tickets	496824d7-63ad-427b-b648-3357ccd20b83	\N	{"Prioridad": "MEDIA"}	{"Prioridad": "CRITICA"}	\N	\N	\N	2026-07-17 20:13:26.563569+00
b73300dc-4cc4-4160-9081-be95c4854767	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	Usuario	02419c75-3006-4f51-8019-a435201f52ba	\N	\N	{"Rol": "USUARIO", "Correo": "luismalito@gmail.com", "EmpresaId": "1a3ef008-e768-43f3-9bbd-0be675710cf6", "SucursalId": "1330cb6d-2785-488e-8710-adcb6d2fceb7", "NombreCompleto": "Luis Malito"}	\N	\N	\N	2026-07-17 20:41:30.095064+00
289c6a66-d6b2-4494-a76f-9d0f2c40916c	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	Usuario	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	\N	\N	{"Rol": "TRABAJADOR", "Correo": "geancarlosbarrionuevo@outlook.com", "EmpresaId": "1a3ef008-e768-43f3-9bbd-0be675710cf6", "SucursalId": "1330cb6d-2785-488e-8710-adcb6d2fceb7", "NombreCompleto": "Geancarlos Barrionuevo"}	\N	\N	\N	2026-07-17 20:41:56.34372+00
7e60cd48-4b83-4dd1-bb3b-e521680dbec1	02419c75-3006-4f51-8019-a435201f52ba	Sistema	SISTEMA	CREAR	sistema	tickets	e44d4b6c-f9de-4b10-926d-8eb418d445a5	\N	\N	{"Estado": 1, "Titulo": "error de el ticket", "SolicitanteId": "02419c75-3006-4f51-8019-a435201f52ba"}	\N	\N	\N	2026-07-17 21:10:22.298093+00
43107b5c-da0e-41c5-892b-c32c528e0660	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ASIGNAR	sistema	tickets	e44d4b6c-f9de-4b10-926d-8eb418d445a5	\N	{"Estado": "SIN_ASIGNAR", "TecnicoId": null}	{"Estado": "ASIGNADO", "TecnicoId": "7eaf59d2-415a-4297-ae8a-032d1ab8d2ea"}	\N	\N	\N	2026-07-17 21:13:10.038826+00
2ddc84ee-e6b6-441b-9b09-d86cf1cbf292	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	INICIAR_PROCESO	sistema	tickets	e44d4b6c-f9de-4b10-926d-8eb418d445a5	\N	{"Estado": "ASIGNADO"}	{"Estado": "EN_PROCESO"}	\N	\N	\N	2026-07-17 21:15:15.902815+00
ca481942-6b60-4385-bb20-5cc56b066cf9	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	tickets	62b633a8-4a8b-4bca-9d00-8f098c6b2452	\N	\N	{"Estado": 1, "Titulo": "prueba de titulo", "SolicitanteId": "21540103-61cc-4141-a3f1-11763957b648"}	\N	\N	\N	2026-07-18 18:14:24.145016+00
979115c0-7496-4d83-b913-b704d3407fbe	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	tickets	e6bc53e4-6fd0-4965-b7c5-bcf79185bb05	\N	\N	{"Estado": 1, "Titulo": "ads", "SolicitanteId": "21540103-61cc-4141-a3f1-11763957b648"}	\N	\N	\N	2026-07-20 20:32:41.430797+00
09ea458a-f288-412a-a2b3-eb0adf666e83	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	tickets	8ba5f1be-4ce8-434d-be41-3f6edcc59e4f	\N	\N	{"Estado": 1, "Titulo": null, "SolicitanteId": "21540103-61cc-4141-a3f1-11763957b648"}	\N	\N	\N	2026-07-20 22:02:04.133382+00
3fd15a85-7fce-4eab-88b9-24a763011f03	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR_DATOS	sistema	tickets	8ba5f1be-4ce8-434d-be41-3f6edcc59e4f	\N	{"Titulo": "", "Ubicacion": "", "Descripcion": "", "TipoServicioId": "641ca311-3280-4faa-ac49-285822a16f02"}	{"Titulo": "asd", "Ubicacion": "", "Descripcion": "dasd", "TipoServicioId": "641ca311-3280-4faa-ac49-285822a16f02"}	\N	\N	\N	2026-07-20 22:08:28.968075+00
08008f74-402c-4710-995b-a9f20435323c	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	parametros	071993c2-6c6f-490f-a895-6594762ad001	\N	{"Valor": ""}	{"Valor": "false"}	\N	\N	\N	2026-07-21 03:20:48.062152+00
70ca7a27-5a9f-4098-88ce-ca867a2fa16f	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	parametros	5b9c4257-fcf9-4238-adad-aad622a78c96	\N	{"Valor": ""}	{"Valor": "60"}	\N	\N	\N	2026-07-21 03:20:48.156015+00
6f282b14-81f4-4c9a-a996-fe26974a6f5f	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	parametros	d94cc139-02ee-4782-933c-ba895dfb1d36	\N	{"Valor": ""}	{"Valor": "false"}	\N	\N	\N	2026-07-21 03:20:48.065843+00
2a6e262f-1344-4269-b8bc-cdf33ed38a43	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	parametros	f1a498c7-283e-4933-a523-dcd9165257be	\N	{"Valor": ""}	{"Valor": "true"}	\N	\N	\N	2026-07-21 03:23:29.464034+00
efeaa8da-5d6f-41ba-8909-261e3b5aee12	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	parametros	04af34f7-7d50-4bb7-98a0-00541136a415	\N	{"Valor": ""}	{"Valor": "America/Lima"}	\N	\N	\N	2026-07-21 03:23:29.678843+00
138bbe8d-d0ff-4236-8fca-e0843083c881	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	5b9c4257-fcf9-4238-adad-aad622a78c96	\N	{"Valor": "60"}	{"Valor": "60"}	\N	\N	\N	2026-07-21 03:23:46.667726+00
cefda9aa-f3d8-4566-8580-ff52425042d0	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	071993c2-6c6f-490f-a895-6594762ad001	\N	{"Valor": "false"}	{"Valor": "false"}	\N	\N	\N	2026-07-21 03:23:46.669744+00
d1d180c3-029c-42dc-be82-45df4a3c603e	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	d94cc139-02ee-4782-933c-ba895dfb1d36	\N	{"Valor": "false"}	{"Valor": "false"}	\N	\N	\N	2026-07-21 03:23:46.671213+00
287d9f80-199c-4bc2-b2c8-6c2ee240ef1c	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	parametros	d3414cfa-da15-4503-83de-317c791b78b7	\N	{"Valor": ""}	{"Valor": "false"}	\N	\N	\N	2026-07-21 03:37:08.575755+00
7a516f12-71c4-477f-9995-f7cc80a59753	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	parametros	e1bcbd31-ddcc-42fc-8238-c23cbf4d1a68	\N	{"Valor": ""}	{"Valor": "false"}	\N	\N	\N	2026-07-21 03:37:08.481375+00
d578eb23-5cb9-4ffb-8ce4-b8228058c8fb	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	parametros	a23f5398-b34d-4bb3-8353-cc9282ffa1f3	\N	{"Valor": ""}	{"Valor": "60"}	\N	\N	\N	2026-07-21 03:37:08.576703+00
5b365411-e9f4-4a71-becb-d012f4aeae4b	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	71d6ae87-8db9-4b78-9fb0-3834600d057e	\N	{"Valor": "true"}	{"Valor": "false"}	\N	\N	\N	2026-07-21 03:37:31.5945+00
1beab57b-ff9b-432a-a467-e7c479e3558f	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	parametros	71d6ae87-8db9-4b78-9fb0-3834600d057e	\N	{"Valor": ""}	{"Valor": "true"}	\N	\N	\N	2026-07-21 03:37:22.785465+00
162f60c2-1696-407d-b2a4-965666625707	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	parametros	665c920c-4f74-496e-8edb-70ab856a9d63	\N	{"Valor": ""}	{"Valor": "America/Lima"}	\N	\N	\N	2026-07-21 03:37:22.88616+00
ecbc9aec-ba97-4be9-aab9-0d30188576bd	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	parametros	2ab459ba-8f37-48a4-8d5b-4592556f6639	\N	{"Valor": ""}	{"Valor": "true"}	\N	\N	\N	2026-07-21 03:37:23.880143+00
51c99c99-0bc8-4fd2-879e-ed97c9518a19	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	parametros	cef3c96b-2740-4bfb-94b3-a1c2d0866dde	\N	{"Valor": ""}	{"Valor": "10"}	\N	\N	\N	2026-07-21 03:37:23.885424+00
67db07e3-d049-40c8-8d35-af0c30aad0c7	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	parametros	a5029fda-8b89-4df0-a82d-4f82fda40f13	\N	{"Valor": ""}	{"Valor": "false"}	\N	\N	\N	2026-07-21 03:37:23.982869+00
ac2bdd41-9c6b-4a4b-9d81-a35435b5aecf	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	665c920c-4f74-496e-8edb-70ab856a9d63	\N	{"Valor": "America/Lima"}	{"Valor": "America/Lima"}	\N	\N	\N	2026-07-21 03:37:31.681728+00
db59a299-bf6b-4baa-b676-11bc62984e3b	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	Usuario	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N	{"Rol": "SUPERADMIN", "Correo": "pruebastecnicas2026@gmail.com", "EmpresaId": "1a3ef008-e768-43f3-9bbd-0be675710cf6", "SucursalId": "1330cb6d-2785-488e-8710-adcb6d2fceb7", "NombreCompleto": "Pruebas Tecnicas"}	\N	\N	\N	2026-07-21 14:15:22.096453+00
36612fe1-5447-4c61-aef2-140e9ade7de1	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	CREAR	sistema	tickets	619711dc-e907-4b64-bf75-d64e657a2790	\N	\N	{"Estado": 1, "Titulo": null, "SolicitanteId": "ffced565-d714-42ce-a4f1-995c9511441c"}	\N	\N	\N	2026-07-21 14:29:48.976412+00
33d662d9-4327-4484-8e14-7eeb40a76797	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	ASIGNAR	sistema	tickets	619711dc-e907-4b64-bf75-d64e657a2790	\N	{"Estado": "SIN_ASIGNAR", "TecnicoId": null}	{"Estado": "ASIGNADO", "TecnicoId": "7eaf59d2-415a-4297-ae8a-032d1ab8d2ea"}	\N	\N	\N	2026-07-21 14:30:50.397727+00
054f10ae-2612-4ef4-9e42-57fdf78636ee	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	INICIAR_PROCESO	sistema	tickets	619711dc-e907-4b64-bf75-d64e657a2790	\N	{"Estado": "ASIGNADO"}	{"Estado": "EN_PROCESO"}	\N	\N	\N	2026-07-27 14:54:40.015706+00
5915c97d-daea-4066-b903-0c21b933979d	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	DESACTIVADO	sistema	sucursales	49030017-59bf-4ed0-9fa5-5c19aa2a96f1	\N	{"Activa": true}	{"Activa": false}	\N	\N	\N	2026-07-27 14:59:01.989265+00
c37e301d-7351-4632-b3b1-eb1dfeb6903f	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	ACTIVADO	sistema	sucursales	49030017-59bf-4ed0-9fa5-5c19aa2a96f1	\N	{"Activa": false}	{"Activa": true}	\N	\N	\N	2026-07-27 14:59:06.298148+00
160cf44c-e235-4b49-9322-334f0e08c1d6	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZARSUCURSALES	sistema	UsuarioSucursal	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	\N	\N	{"Sucursales": [{"SucursalId": "1330cb6d-2785-488e-8710-adcb6d2fceb7", "EsPrincipal": true}, {"SucursalId": "a48d3ab2-9057-45a4-980b-ce28ef27f028", "EsPrincipal": false}, {"SucursalId": "07c96055-9c8d-4aa0-8fe6-84cb70c532d2", "EsPrincipal": false}, {"SucursalId": "a48bcd1f-39ca-4071-9c54-7a5f412c3d1a", "EsPrincipal": false}]}	\N	\N	\N	2026-07-27 20:02:42.646172+00
3f2cc46c-6cf2-4686-a727-abbb31ac11db	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZARPERFIL	sistema	Usuario	ffced565-d714-42ce-a4f1-995c9511441c	\N	{"AreaId": null, "Nombre": "Pruebas", "FotoUrl": null, "Apellido": "Tecnicas", "Telefono": null}	{"AreaId": null, "Nombre": "Pruebas", "FotoUrl": null, "Apellido": "Tecnicas", "Telefono": null}	\N	\N	\N	2026-07-27 20:07:18.144582+00
352e2f29-eac8-4a22-aeff-c115d1b66bbd	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CAMBIARROL	sistema	Usuario	ffced565-d714-42ce-a4f1-995c9511441c	\N	{"Rol": "SUPERADMIN"}	{"Rol": "ADMIN"}	\N	\N	\N	2026-07-27 20:07:18.350311+00
72b5ec50-ea51-4e3a-8859-19e3792ce939	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZARSUCURSALES	sistema	UsuarioSucursal	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	\N	\N	{"Sucursales": [{"SucursalId": "1330cb6d-2785-488e-8710-adcb6d2fceb7", "EsPrincipal": true}, {"SucursalId": "07c96055-9c8d-4aa0-8fe6-84cb70c532d2", "EsPrincipal": false}, {"SucursalId": "a48d3ab2-9057-45a4-980b-ce28ef27f028", "EsPrincipal": false}, {"SucursalId": "a48bcd1f-39ca-4071-9c54-7a5f412c3d1a", "EsPrincipal": false}]}	\N	\N	\N	2026-07-27 20:08:59.911423+00
06637dcc-2480-4204-b5c0-98ef4c593c29	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZARPERFIL	sistema	Usuario	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	\N	{"AreaId": null, "Nombre": "Geancarlos", "FotoUrl": null, "Apellido": "Barrionuevo", "Telefono": null}	{"AreaId": null, "Nombre": "Geancarlos", "FotoUrl": null, "Apellido": "Barrionuevo", "Telefono": null}	\N	\N	\N	2026-07-27 20:09:03.948654+00
9f2fdf14-bcea-4749-8ee8-3bf18d6af840	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CAMBIARROL	sistema	Usuario	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	\N	{"Rol": "TRABAJADOR"}	{"Rol": "SUPERADMIN"}	\N	\N	\N	2026-07-27 20:09:25.368382+00
5ca42db7-405f-4edf-ba42-5424598a7699	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZARPERFIL	sistema	Usuario	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	\N	{"AreaId": null, "Nombre": "Geancarlos", "FotoUrl": null, "Apellido": "Barrionuevo", "Telefono": null}	{"AreaId": null, "Nombre": "Geancarlos", "FotoUrl": null, "Apellido": "Barrionuevo", "Telefono": null}	\N	\N	\N	2026-07-27 20:09:44.948205+00
dfdb0c9c-992c-4e98-a64f-07500d42686a	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CAMBIARROL	sistema	Usuario	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	\N	{"Rol": "SUPERADMIN"}	{"Rol": "TRABAJADOR"}	\N	\N	\N	2026-07-27 20:09:52.252387+00
a4c1e3ac-4924-431b-be9e-b0c5f48dfa71	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	CREAR	sistema	parametros	ba983129-c853-4a37-9a99-f8e923b35567	\N	{"Valor": ""}	{"Valor": "Super Administrador"}	\N	\N	\N	2026-07-27 21:19:54.056357+00
26f653a5-b935-4951-be54-d89b3cdcb231	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	CREAR	sistema	parametros	4fc431a6-e613-463f-ad9a-f2fd8a586677	\N	{"Valor": ""}	{"Valor": "Acceso total. Gestiona empresas, usuarios, configuración y catálogos globales."}	\N	\N	\N	2026-07-27 21:19:54.060508+00
df0b0fcf-e416-4a20-b5e7-624b9dc2132a	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	Sistema	SISTEMA	CREAR	sistema	tickets	fb23048f-b94a-41ba-9353-f1754f02fb61	\N	\N	{"Estado": 1, "Titulo": null, "SolicitanteId": "7eaf59d2-415a-4297-ae8a-032d1ab8d2ea"}	\N	\N	\N	2026-07-31 01:33:17.660498+00
d353ae2f-795f-49de-ab7b-e9317bfb99ee	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ASIGNAR	sistema	tickets	fb23048f-b94a-41ba-9353-f1754f02fb61	\N	{"Estado": "SIN_ASIGNAR", "TecnicoId": null}	{"Estado": "ASIGNADO", "TecnicoId": "7eaf59d2-415a-4297-ae8a-032d1ab8d2ea"}	\N	\N	\N	2026-07-31 01:35:03.573901+00
d5b665d2-f6bf-4ff1-8b43-301f2506c0dc	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	665c920c-4f74-496e-8edb-70ab856a9d63	\N	{"Valor": "America/Lima"}	{"Valor": "America/Lima"}	\N	\N	\N	2026-08-01 01:15:50.359387+00
2818823f-d4a4-4bb5-bacc-89739503774b	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	71d6ae87-8db9-4b78-9fb0-3834600d057e	\N	{"Valor": "false"}	{"Valor": "true"}	\N	\N	\N	2026-08-01 01:15:50.35938+00
aab634fb-7608-48d5-a231-b19cb53b8e23	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	d3414cfa-da15-4503-83de-317c791b78b7	\N	{"Valor": "false"}	{"Valor": "true"}	\N	\N	\N	2026-08-01 01:15:59.044771+00
eb465c76-93a3-4a1b-98eb-d7294b403df0	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	a23f5398-b34d-4bb3-8353-cc9282ffa1f3	\N	{"Valor": "60"}	{"Valor": "60"}	\N	\N	\N	2026-08-01 01:15:59.052327+00
2a501748-c82f-4a3e-97de-8f8bac30df73	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	e1bcbd31-ddcc-42fc-8238-c23cbf4d1a68	\N	{"Valor": "false"}	{"Valor": "true"}	\N	\N	\N	2026-08-01 01:15:59.052637+00
9f8434ae-17b0-430b-b630-a425c224d39f	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	a23f5398-b34d-4bb3-8353-cc9282ffa1f3	\N	{"Valor": "60"}	{"Valor": "120"}	\N	\N	\N	2026-08-01 01:16:13.051012+00
7a40eb0e-ff6b-4323-86f2-233e5d682b25	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	e1bcbd31-ddcc-42fc-8238-c23cbf4d1a68	\N	{"Valor": "true"}	{"Valor": "true"}	\N	\N	\N	2026-08-01 01:16:13.147499+00
2abca30a-2a8a-4623-9673-64e6f2b8f84b	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	d3414cfa-da15-4503-83de-317c791b78b7	\N	{"Valor": "true"}	{"Valor": "true"}	\N	\N	\N	2026-08-01 01:16:13.144557+00
599b28f6-6641-4171-ad3f-0c848bf1cadb	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	2ab459ba-8f37-48a4-8d5b-4592556f6639	\N	{"Valor": "true"}	{"Valor": "true"}	\N	\N	\N	2026-08-01 01:16:24.447839+00
f19699ce-970b-4a40-a863-8b112148b134	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	a5029fda-8b89-4df0-a82d-4f82fda40f13	\N	{"Valor": "false"}	{"Valor": "true"}	\N	\N	\N	2026-08-01 01:16:24.3477+00
17dbd753-4c30-43d6-9c2f-d3c66f8c8890	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	cef3c96b-2740-4bfb-94b3-a1c2d0866dde	\N	{"Valor": "10"}	{"Valor": "5"}	\N	\N	\N	2026-08-01 01:16:24.449533+00
b3b2f500-5190-4529-920b-65735e3b7ac6	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	parametros	ddba9729-7865-4ba5-93da-e60a1e07943c	\N	{"Valor": ""}	{"Valor": "true"}	\N	\N	\N	2026-08-01 01:17:04.552567+00
d55c9350-7cc3-4ee0-a565-43436bdffb75	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	parametros	39a6f4b6-ccd9-49f7-9482-d379522e4659	\N	{"Valor": ""}	{"Valor": "true"}	\N	\N	\N	2026-08-01 01:17:04.555888+00
936e5b63-5c39-41da-92ac-f1e9c5b8fe9f	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	parametros	08e8b9a3-48be-4713-82fb-a8e85f5c2d71	\N	{"Valor": ""}	{"Valor": "false"}	\N	\N	\N	2026-08-01 01:17:04.644392+00
52b3ac4f-3c42-4e84-8c0e-fc80fdda01ee	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	CREAR	sistema	parametros	da089abd-84f1-4296-9266-bee123ffde80	\N	{"Valor": ""}	{"Valor": "true"}	\N	\N	\N	2026-08-01 01:17:04.655938+00
fabbd231-2ade-4dc2-80f1-6c7c4bbd1b7c	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	71d6ae87-8db9-4b78-9fb0-3834600d057e	\N	{"Valor": "true"}	{"Valor": "false"}	\N	\N	\N	2026-08-01 18:01:01.499005+00
8da08234-2436-43f6-a0e6-ba4dfd0a5220	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	2ab459ba-8f37-48a4-8d5b-4592556f6639	\N	{"Valor": "true"}	{"Valor": "true"}	\N	\N	\N	2026-08-01 18:01:02.70743+00
74f49f82-ef98-48a1-9f6e-ee4f9d46e91e	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	2ab459ba-8f37-48a4-8d5b-4592556f6639	\N	{"Valor": "true"}	{"Valor": "false"}	\N	\N	\N	2026-08-01 18:01:04.517738+00
7c456079-6a2a-4587-9138-6f12b71a7fdf	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	CREAR	sistema	tickets	487f9484-9b26-42b8-8439-395d9625fe19	\N	\N	{"Estado": 1, "Titulo": "soporte tecnico prueba", "SolicitanteId": "ffced565-d714-42ce-a4f1-995c9511441c"}	\N	\N	\N	2026-08-03 14:36:27.90368+00
62868a33-2846-4f06-a45b-af096147e2d0	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	ASIGNAR	sistema	tickets	487f9484-9b26-42b8-8439-395d9625fe19	\N	{"Estado": "SIN_ASIGNAR", "TecnicoId": null}	{"Estado": "ASIGNADO", "TecnicoId": "7eaf59d2-415a-4297-ae8a-032d1ab8d2ea"}	\N	\N	\N	2026-08-03 14:53:35.423048+00
7d91d75c-c47b-4638-a1a4-6ca5af35bd0c	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	INICIAR_PROCESO	sistema	tickets	487f9484-9b26-42b8-8439-395d9625fe19	\N	{"Estado": "ASIGNADO"}	{"Estado": "EN_PROCESO"}	\N	\N	\N	2026-08-03 15:48:52.815258+00
ca18ee5b-9947-43b8-b540-bed64a8e31a5	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	CREAR	sistema	Usuario	25541c55-aafe-4714-98d1-a177b057302e	\N	\N	{"Rol": "USUARIO", "Correo": "rclaudianatalia@gmail.com", "EmpresaId": "1a3ef008-e768-43f3-9bbd-0be675710cf6", "SucursalId": "d7459f2e-2bdc-4d31-8f7d-ca37f3732706", "NombreCompleto": "Claudia Natalia Romero Andrade"}	\N	\N	\N	2026-08-03 15:57:41.20461+00
24a28ddb-f094-4324-aa8d-17a13082d2ef	25541c55-aafe-4714-98d1-a177b057302e	Sistema	SISTEMA	CREAR	sistema	tickets	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	\N	{"Estado": 1, "Titulo": "nuevo ticket de prueba", "SolicitanteId": "25541c55-aafe-4714-98d1-a177b057302e"}	\N	\N	\N	2026-08-03 16:15:59.699247+00
5c8ede19-2fef-4152-a758-5bc2db99756f	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	ASIGNAR	sistema	tickets	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	{"Estado": "SIN_ASIGNAR", "TecnicoId": null}	{"Estado": "ASIGNADO", "TecnicoId": "7eaf59d2-415a-4297-ae8a-032d1ab8d2ea"}	\N	\N	\N	2026-08-03 16:19:14.303206+00
4f9a4e9b-aa26-4b49-9cfa-5744da9445ad	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	INICIAR_PROCESO	sistema	tickets	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	{"Estado": "ASIGNADO"}	{"Estado": "EN_PROCESO"}	\N	\N	\N	2026-08-03 16:20:59.603217+00
2e1eb2c3-1593-43c5-94e7-fa4da30a2aef	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	SUBMIT_VALIDACION	sistema	tickets	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	{"Estado": "EN_PROCESO"}	{"Estado": "PENDIENTE_VALIDACION"}	\N	\N	\N	2026-08-03 16:24:20.11912+00
5c42040d-9689-4ae0-ab84-51cf91e2a057	25541c55-aafe-4714-98d1-a177b057302e	Sistema	SISTEMA	REABRIR	sistema	tickets	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	{"Estado": "PENDIENTE_VALIDACION"}	{"Estado": "REABIERTO", "MotivoRechazoId": "d6ab1693-9f5a-4f6b-a9ca-4ebdc02828ea", "ComentarioRechazo": "hola"}	\N	\N	\N	2026-08-03 19:51:58.165714+00
d22c3cb3-1edd-4725-adb9-e81754c90994	25541c55-aafe-4714-98d1-a177b057302e	Sistema	SISTEMA	CREAR	sistema	tickets	e3842e17-5cfc-4cbb-a504-010a3293a55a	\N	\N	{"Estado": 1, "Titulo": "tickedt de prueba de usuario", "SolicitanteId": "25541c55-aafe-4714-98d1-a177b057302e"}	\N	\N	\N	2026-08-03 20:50:27.878681+00
f7e86eb1-a6b1-40c2-8f73-c8c41d7e08bc	25541c55-aafe-4714-98d1-a177b057302e	Sistema	SISTEMA	CANCELAR	sistema	tickets	e3842e17-5cfc-4cbb-a504-010a3293a55a	\N	{"Estado": "SIN_ASIGNAR"}	{"Estado": "CANCELADO", "MotivoCancelacionId": "e8205d1b-6fc0-490c-ad65-f7d48a152fcc"}	\N	\N	\N	2026-08-03 20:52:40.880323+00
3ab4b8bb-d7ab-420f-956d-96c8ef1d7710	25541c55-aafe-4714-98d1-a177b057302e	Sistema	SISTEMA	CREAR	sistema	tickets	26bb057a-9c13-436f-9d2b-1178aa1ad034	\N	\N	{"Estado": 1, "Titulo": "ticket de prueba 4", "SolicitanteId": "25541c55-aafe-4714-98d1-a177b057302e"}	\N	\N	\N	2026-08-03 21:00:47.584212+00
b67cfbe8-3d26-4a8b-a86d-aeb3f34f8dc5	25541c55-aafe-4714-98d1-a177b057302e	Sistema	SISTEMA	CANCELAR	sistema	tickets	26bb057a-9c13-436f-9d2b-1178aa1ad034	\N	{"Estado": "SIN_ASIGNAR"}	{"Estado": "CANCELADO", "MotivoCancelacionId": "e8205d1b-6fc0-490c-ad65-f7d48a152fcc"}	\N	\N	\N	2026-08-03 21:08:17.204593+00
aa053df4-1699-4cd0-a485-401cd454e493	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	ACTUALIZAR_DATOS	sistema	tickets	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	{"Titulo": "nuevo ticket de prueba", "Ubicacion": "", "Descripcion": "nuevo ticket de prueba nueva 1", "TipoServicioId": "641ca311-3280-4faa-ac49-285822a16f02"}	{"Titulo": "nuevo ticket de prueba8", "Ubicacion": "", "Descripcion": "nuevo ticket de prueba nueva 2", "TipoServicioId": "641ca311-3280-4faa-ac49-285822a16f02"}	\N	\N	\N	2026-08-03 21:27:34.479401+00
495a53c7-6f9b-46ff-82e4-355d8c43416c	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	f1a498c7-283e-4933-a523-dcd9165257be	\N	{"Valor": "true"}	{"Valor": "true"}	\N	\N	\N	2026-08-03 21:40:16.977842+00
83298bce-eb45-47dc-8c59-455bdbbf18f4	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	CANCELAR	sistema	tickets	487f9484-9b26-42b8-8439-395d9625fe19	\N	{"Estado": "EN_PROCESO"}	{"Estado": "CANCELADO", "MotivoCancelacionId": "e8205d1b-6fc0-490c-ad65-f7d48a152fcc"}	\N	\N	\N	2026-08-04 01:17:19.014459+00
561f1323-151c-4345-a7ee-65d4371133be	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	CANCELAR	sistema	tickets	8ba5f1be-4ce8-434d-be41-3f6edcc59e4f	\N	{"Estado": "SIN_ASIGNAR"}	{"Estado": "CANCELADO", "MotivoCancelacionId": "e8205d1b-6fc0-490c-ad65-f7d48a152fcc"}	\N	\N	\N	2026-08-04 01:18:04.840391+00
486e84fc-56ca-45a4-9603-4a794a012cba	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	CREAR	sistema	tickets	d9cb455e-d34a-4ef1-bf19-14068044a85a	\N	\N	{"Estado": 1, "Titulo": "ticket de prueba de usuario 1", "SolicitanteId": "ffced565-d714-42ce-a4f1-995c9511441c"}	\N	\N	\N	2026-08-04 16:04:27.274109+00
3753d351-7fad-48a1-8fd7-cb7cc1da7ed0	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	ASIGNAR	sistema	tickets	d9cb455e-d34a-4ef1-bf19-14068044a85a	\N	{"Estado": "SIN_ASIGNAR", "TecnicoId": null}	{"Estado": "ASIGNADO", "TecnicoId": "7eaf59d2-415a-4297-ae8a-032d1ab8d2ea"}	\N	\N	\N	2026-08-04 16:09:19.695189+00
aa81926d-02b3-4518-a552-cd2a3c951820	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	INICIAR_PROCESO	sistema	tickets	d9cb455e-d34a-4ef1-bf19-14068044a85a	\N	{"Estado": "ASIGNADO"}	{"Estado": "EN_PROCESO"}	\N	\N	\N	2026-08-04 16:10:07.422569+00
c8c7b183-80a0-45eb-8374-dee27d51cd1a	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	PAUSE_ESPERA	sistema	tickets	d9cb455e-d34a-4ef1-bf19-14068044a85a	\N	{"Estado": "EN_PROCESO"}	{"Estado": "EN_ESPERA"}	\N	\N	\N	2026-08-04 16:16:34.273774+00
9d69bb46-08fe-4239-a1cf-376b9712796f	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	REANUDAR_ESPERA	sistema	tickets	d9cb455e-d34a-4ef1-bf19-14068044a85a	\N	{"Estado": "EN_ESPERA"}	{"Estado": "EN_PROCESO"}	\N	\N	\N	2026-08-04 16:16:39.784016+00
675476a1-fd6c-4164-a9d7-3a377bc6d400	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	SUBMIT_VALIDACION	sistema	tickets	d9cb455e-d34a-4ef1-bf19-14068044a85a	\N	{"Estado": "EN_PROCESO"}	{"Estado": "PENDIENTE_VALIDACION"}	\N	\N	\N	2026-08-04 16:16:47.692822+00
f12ae378-4ab6-4428-bddd-5535dbe3dc32	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	CREAR	sistema	Usuario	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	\N	\N	{"Rol": "TRABAJADOR", "Correo": "claromeroa@uch.pe", "EmpresaId": "1a3ef008-e768-43f3-9bbd-0be675710cf6", "SucursalId": "d7459f2e-2bdc-4d31-8f7d-ca37f3732706", "NombreCompleto": "Natalia Romero"}	\N	\N	\N	2026-08-04 17:01:48.213293+00
d7ed0a3f-d4a4-4b11-b606-4ed5560ff193	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	Sistema	SISTEMA	CREAR	sistema	tickets	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	\N	\N	{"Estado": 1, "Titulo": "ticket de prueba 23", "SolicitanteId": "a72dcddd-405e-4d93-a819-d6f16bfc5f1c"}	\N	\N	\N	2026-08-04 17:07:08.714169+00
fa4d7856-bde0-4b9d-b250-8e9f5b6f97a8	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	ASIGNAR	sistema	tickets	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	\N	{"Estado": "SIN_ASIGNAR", "TecnicoId": null}	{"Estado": "ASIGNADO", "TecnicoId": "a72dcddd-405e-4d93-a819-d6f16bfc5f1c"}	\N	\N	\N	2026-08-04 17:07:46.508728+00
25732aed-7d54-41d9-8404-3ddf9a47a5f0	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	INICIAR_PROCESO	sistema	tickets	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	\N	{"Estado": "ASIGNADO"}	{"Estado": "EN_PROCESO"}	\N	\N	\N	2026-08-04 17:16:48.337145+00
da45ae5e-cde8-401d-9c4d-3cc57ab31848	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	PAUSE_ESPERA	sistema	tickets	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	\N	{"Estado": "EN_PROCESO"}	{"Estado": "EN_ESPERA"}	\N	\N	\N	2026-08-04 17:16:55.925639+00
0f9ada83-d040-4055-a3cc-4c62b9dfca12	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	REANUDAR_ESPERA	sistema	tickets	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	\N	{"Estado": "EN_ESPERA"}	{"Estado": "EN_PROCESO"}	\N	\N	\N	2026-08-04 17:17:02.745628+00
4ff2b6bf-ce03-4baf-a926-3ec28ae3e756	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	SUBMIT_VALIDACION	sistema	tickets	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	\N	{"Estado": "EN_PROCESO"}	{"Estado": "PENDIENTE_VALIDACION"}	\N	\N	\N	2026-08-04 17:17:16.906584+00
4ed77821-903f-4dc7-852a-c6d515013744	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	71d6ae87-8db9-4b78-9fb0-3834600d057e	\N	{"Valor": "false"}	{"Valor": "true"}	\N	\N	\N	2026-08-06 19:27:14.170315+00
413019d4-586f-49b7-afaa-3d490a3ec885	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	71d6ae87-8db9-4b78-9fb0-3834600d057e	\N	{"Valor": "true"}	{"Valor": "true"}	\N	\N	\N	2026-08-06 19:27:15.072429+00
d87d5743-e497-4c0f-baa8-4bf4046dc773	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ACTUALIZAR	sistema	parametros	71d6ae87-8db9-4b78-9fb0-3834600d057e	\N	{"Valor": "true"}	{"Valor": "false"}	\N	\N	\N	2026-08-06 19:48:34.5056+00
e857dea1-5271-4004-a34b-7157d51324a9	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ASIGNAR	sistema	tickets	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	{"Estado": "REABIERTO", "TecnicoId": null}	{"Estado": "ASIGNADO", "TecnicoId": "7eaf59d2-415a-4297-ae8a-032d1ab8d2ea"}	\N	\N	\N	2026-08-06 19:50:37.228539+00
33896fe3-cf99-4a03-bf09-6d4166bfc782	21540103-61cc-4141-a3f1-11763957b648	Sistema	SISTEMA	ASIGNAR	sistema	tickets	62b633a8-4a8b-4bca-9d00-8f098c6b2452	\N	{"Estado": "SIN_ASIGNAR", "TecnicoId": null}	{"Estado": "ASIGNADO", "TecnicoId": "7eaf59d2-415a-4297-ae8a-032d1ab8d2ea"}	\N	\N	\N	2026-08-06 19:51:26.647505+00
24c28d89-132a-4b89-97f0-1d856d19da98	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	CREAR	sistema	tickets	f856d580-0907-4f8f-ae86-7007cf8d527b	\N	\N	{"Estado": 1, "Titulo": "ticket de prueba", "SolicitanteId": "ffced565-d714-42ce-a4f1-995c9511441c"}	\N	\N	\N	2026-08-12 13:42:10.714588+00
30e32514-d467-4bd9-b2ff-5b2530de0cb7	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	ASIGNAR	sistema	tickets	f856d580-0907-4f8f-ae86-7007cf8d527b	\N	{"Estado": "SIN_ASIGNAR", "TecnicoId": null}	{"Estado": "ASIGNADO", "TecnicoId": "a72dcddd-405e-4d93-a819-d6f16bfc5f1c"}	\N	\N	\N	2026-08-12 13:44:03.910749+00
774678fe-0094-4814-b3c3-f871ec27bcdd	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	INICIAR_PROCESO	sistema	tickets	f856d580-0907-4f8f-ae86-7007cf8d527b	\N	{"Estado": "ASIGNADO"}	{"Estado": "EN_PROCESO"}	\N	\N	\N	2026-08-12 14:03:38.732656+00
0512a2cc-2cf8-4207-b575-17da397e3b92	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	SUBMIT_VALIDACION	sistema	tickets	f856d580-0907-4f8f-ae86-7007cf8d527b	\N	{"Estado": "EN_PROCESO"}	{"Estado": "PENDIENTE_VALIDACION"}	\N	\N	\N	2026-08-12 14:06:14.442926+00
dc85dd21-6893-4ddd-9b4c-8772f4d0b45c	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	CREAR	sistema	tickets	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	\N	{"Estado": 1, "Titulo": "ticket de prueba 23", "SolicitanteId": "ffced565-d714-42ce-a4f1-995c9511441c"}	\N	\N	\N	2026-08-12 14:26:55.126389+00
c16b7407-f375-40df-8c32-356a28e5be7d	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	ASIGNAR	sistema	tickets	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	{"Estado": "SIN_ASIGNAR", "TecnicoId": null}	{"Estado": "ASIGNADO", "TecnicoId": "a72dcddd-405e-4d93-a819-d6f16bfc5f1c"}	\N	\N	\N	2026-08-12 14:27:30.940093+00
4fac7597-f3db-404f-9f38-f865f265a681	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	ACTIVADO	sistema	tipos_servicio	08279f43-36da-491f-bb06-256f388add5a	\N	{"Activo": false}	{"Activo": true}	\N	\N	\N	2026-08-12 14:44:42.224604+00
29ba7f00-3a84-44e4-a1d5-a4bab64a9e68	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	DESACTIVADO	sistema	tipos_servicio	08279f43-36da-491f-bb06-256f388add5a	\N	{"Activo": true}	{"Activo": false}	\N	\N	\N	2026-08-12 14:45:31.82339+00
6bdf6c9e-75b6-4e63-84ac-f3f464992481	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	INICIAR_PROCESO	sistema	tickets	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	{"Estado": "ASIGNADO"}	{"Estado": "EN_PROCESO"}	\N	\N	\N	2026-08-17 14:54:38.439791+00
a3dd7ef9-90f5-4e15-8c54-0548090e108b	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	PAUSE_ESPERA	sistema	tickets	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	{"Estado": "EN_PROCESO"}	{"Estado": "EN_ESPERA"}	\N	\N	\N	2026-08-17 14:55:26.545644+00
adc4d8da-7ba6-403e-9957-36eb1b872da0	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	REANUDAR_ESPERA	sistema	tickets	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	{"Estado": "EN_ESPERA"}	{"Estado": "EN_PROCESO"}	\N	\N	\N	2026-08-17 14:55:35.433183+00
0726a550-b3bc-45e9-8d26-220e54f56e9a	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	SUBMIT_VALIDACION	sistema	tickets	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	{"Estado": "EN_PROCESO"}	{"Estado": "PENDIENTE_VALIDACION"}	\N	\N	\N	2026-08-17 14:55:43.442376+00
038a0ba2-59e4-4d72-8c8f-c977bafbf56b	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	REABRIR	sistema	tickets	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	{"Estado": "PENDIENTE_VALIDACION"}	{"Estado": "REABIERTO", "MotivoRechazoId": "d6ab1693-9f5a-4f6b-a9ca-4ebdc02828ea", "ComentarioRechazo": "e"}	\N	\N	\N	2026-08-17 14:58:30.4666+00
66bee2d7-02a7-4609-9136-01e79dcccdc1	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	CREAR	sistema	tickets	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	\N	{"Estado": 1, "Titulo": "tickte de pruebas 18 de agosto", "SolicitanteId": "ffced565-d714-42ce-a4f1-995c9511441c"}	\N	\N	\N	2026-08-18 17:42:55.826328+00
b63fdc56-1a5e-422c-8463-df32513e5a20	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	ACTUALIZARPERFIL	sistema	Usuario	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	\N	{"AreaId": null, "Nombre": "Natalia", "FotoUrl": null, "Apellido": "Romero", "Telefono": "+51973987140"}	{"AreaId": null, "Nombre": "Natalia", "FotoUrl": null, "Apellido": "Romero", "Telefono": "+51973987140"}	\N	\N	\N	2026-08-18 17:50:11.620999+00
5c276ce9-90a8-479d-aaed-e6a511f150fa	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	ACTUALIZARSUCURSALES	sistema	UsuarioSucursal	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	\N	\N	{"Sucursales": [{"SucursalId": "d7459f2e-2bdc-4d31-8f7d-ca37f3732706", "EsPrincipal": true}, {"SucursalId": "a48d3ab2-9057-45a4-980b-ce28ef27f028", "EsPrincipal": false}]}	\N	\N	\N	2026-08-18 17:50:59.661242+00
28649b44-2e28-4de1-a5bb-b79f3b237434	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	ACTUALIZARPERFIL	sistema	Usuario	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	\N	{"AreaId": null, "Nombre": "Natalia", "FotoUrl": null, "Apellido": "Romero", "Telefono": "+51973987140"}	{"AreaId": null, "Nombre": "Natalia", "FotoUrl": null, "Apellido": "Romero", "Telefono": "+51973987140"}	\N	\N	\N	2026-08-18 17:51:04.53964+00
2a7e15ba-172d-49f9-a59f-0ffd235d8e7e	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	ASIGNAR	sistema	tickets	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	{"Estado": "SIN_ASIGNAR", "TecnicoId": null}	{"Estado": "ASIGNADO", "TecnicoId": "a72dcddd-405e-4d93-a819-d6f16bfc5f1c"}	\N	\N	\N	2026-08-18 17:51:20.720643+00
ed78ab5e-6890-4a4b-9882-a48cea645ce7	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	INICIAR_PROCESO	sistema	tickets	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	{"Estado": "ASIGNADO"}	{"Estado": "EN_PROCESO"}	\N	\N	\N	2026-08-18 19:37:24.605942+00
9b7e1c0b-39f1-4f1b-b8d1-4654feb6f014	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	SUBMIT_VALIDACION	sistema	tickets	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	{"Estado": "EN_PROCESO"}	{"Estado": "PENDIENTE_VALIDACION"}	\N	\N	\N	2026-08-18 19:38:43.928421+00
d7d3b569-8a88-47cc-a41c-352ddc695434	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	CERRAR	sistema	tickets	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	{"Estado": "PENDIENTE_VALIDACION"}	{"Estado": "CERRADO", "Valoracion": null}	\N	\N	\N	2026-08-18 19:45:58.998453+00
1795f160-d332-427a-bc9e-0434a323681d	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	ASIGNAR	sistema	tickets	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	{"Estado": "REABIERTO", "TecnicoId": null}	{"Estado": "ASIGNADO", "TecnicoId": "7eaf59d2-415a-4297-ae8a-032d1ab8d2ea"}	\N	\N	\N	2026-08-18 19:51:16.915493+00
b5a525f5-beab-4829-a5ff-939e6daab116	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	INICIAR_PROCESO	sistema	tickets	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	{"Estado": "ASIGNADO"}	{"Estado": "EN_PROCESO"}	\N	\N	\N	2026-08-18 19:51:26.125044+00
ddfe3f5e-1dc1-450c-9975-b894f62a8cb7	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	PAUSE_ESPERA	sistema	tickets	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	{"Estado": "EN_PROCESO"}	{"Estado": "EN_ESPERA"}	\N	\N	\N	2026-08-18 19:51:37.215887+00
3cc656cb-3074-45a1-bb91-c759cb4564b2	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	REANUDAR_ESPERA	sistema	tickets	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	{"Estado": "EN_ESPERA"}	{"Estado": "EN_PROCESO"}	\N	\N	\N	2026-08-18 19:52:44.316405+00
ac3f85cb-7105-4892-b773-3ec57cd9bc1b	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	SUBMIT_VALIDACION	sistema	tickets	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	{"Estado": "EN_PROCESO"}	{"Estado": "PENDIENTE_VALIDACION"}	\N	\N	\N	2026-08-18 19:54:31.520656+00
dc36533b-42e1-4799-9b1d-e74a1904a592	ffced565-d714-42ce-a4f1-995c9511441c	Sistema	SISTEMA	REABRIR	sistema	tickets	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	{"Estado": "PENDIENTE_VALIDACION"}	{"Estado": "REABIERTO", "MotivoRechazoId": "604261f8-ae9b-4c9f-b6d7-bccb97cf6732", "ComentarioRechazo": "hola"}	\N	\N	\N	2026-08-18 19:55:36.097143+00
\.


ALTER TABLE public.audit_logs ENABLE TRIGGER ALL;

--
-- Data for Name: categorias; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.categorias DISABLE TRIGGER ALL;

COPY public.categorias (id, empresa_id, nombre, descripcion, activa, created_at, updated_at, created_by, updated_by, deleted_at, deleted_by) FROM stdin;
fc80bf18-53f6-4a9b-b486-2dae9861c2c8	\N	Tecnología (TI)	Atención de incidencias relacionadas con equipos, software, redes y sistemas informáticos.	t	2026-07-17 18:57:08.843853+00	2026-07-17 18:57:08.843853+00	21540103-61cc-4141-a3f1-11763957b648	\N	\N	\N
10c2eeb5-e1b3-4d28-9efb-7eb07be2b8e8	\N	Infraestructura	Gestión del mantenimiento y funcionamiento de las instalaciones físicas de la empresa.	t	2026-07-17 18:57:18.070564+00	2026-07-17 18:57:18.070564+00	21540103-61cc-4141-a3f1-11763957b648	\N	\N	\N
2e162bf3-1818-4608-8c4c-d199b46f68ba	\N	Administración	Solicitudes relacionadas con procesos administrativos, documentación y soporte interno.	t	2026-07-17 18:57:26.369404+00	2026-07-17 18:57:26.369404+00	21540103-61cc-4141-a3f1-11763957b648	\N	\N	\N
c4cb8565-fe4f-4222-a4b9-97e74ee70edd	\N	Operaciones	Incidencias asociadas a la operación diaria de las sedes, inmuebles o procesos operativos.	t	2026-07-17 18:57:34.753327+00	2026-07-17 18:57:34.753327+00	21540103-61cc-4141-a3f1-11763957b648	\N	\N	\N
1b567db9-9f99-4c67-86ab-4885deb216dc	\N	Servicios Generales	Atención de requerimientos de mantenimiento, limpieza, mobiliario y apoyo logístico.	t	2026-07-17 18:57:42.650843+00	2026-07-17 18:57:42.650843+00	21540103-61cc-4141-a3f1-11763957b648	\N	\N	\N
\.


ALTER TABLE public.categorias ENABLE TRIGGER ALL;

--
-- Data for Name: correos_guardados; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.correos_guardados DISABLE TRIGGER ALL;

COPY public.correos_guardados (id, usuario_id, correo, created_at) FROM stdin;
\.


ALTER TABLE public.correos_guardados ENABLE TRIGGER ALL;

--
-- Data for Name: empresa_correos_copia; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.empresa_correos_copia DISABLE TRIGGER ALL;

COPY public.empresa_correos_copia (id, empresa_id, correo, activo, created_at) FROM stdin;
0f72b3d0-de93-41b9-a9cf-ba727a2208b7	1a3ef008-e768-43f3-9bbd-0be675710cf6	atencionalcliente@inmoveg.pe	t	2026-07-24 14:53:32.831004+00
ee36bde9-2816-4ec0-b2dd-aa770b143157	1a3ef008-e768-43f3-9bbd-0be675710cf6	cordova.c@inmoveg.pe	t	2026-07-24 14:53:32.831004+00
f03bdff1-2374-4103-9560-f5cc59fecd2f	1a3ef008-e768-43f3-9bbd-0be675710cf6	anticona.e@inmoveg.pe	t	2026-07-24 14:53:32.831004+00
\.


ALTER TABLE public.empresa_correos_copia ENABLE TRIGGER ALL;

--
-- Data for Name: feriados; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.feriados DISABLE TRIGGER ALL;

COPY public.feriados (id, empresa_id, pais_iso, fecha, nombre, recurrente, created_at, created_by, deleted_at) FROM stdin;
\.


ALTER TABLE public.feriados ENABLE TRIGGER ALL;

--
-- Data for Name: motivos_cancelacion; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.motivos_cancelacion DISABLE TRIGGER ALL;

COPY public.motivos_cancelacion (id, empresa_id, texto, activo, created_at, updated_at, created_by, updated_by, deleted_at, deleted_by) FROM stdin;
e8205d1b-6fc0-490c-ad65-f7d48a152fcc	\N	Resuelto por otro medio	t	2026-07-13 14:32:54.650882+00	2026-07-13 14:32:54.650882+00	\N	\N	\N	\N
48805218-87b2-4cb3-9f8d-5434fa79ddee	\N	Ticket duplicado	t	2026-07-13 14:32:54.650882+00	2026-07-13 14:32:54.650882+00	\N	\N	\N	\N
0561c539-1695-4967-a4a7-cd249a31b97b	\N	Solicitud incorrecta o fuera de alcance	t	2026-07-13 14:32:54.650882+00	2026-07-13 14:32:54.650882+00	\N	\N	\N	\N
c23c31fd-5f83-452c-9ace-294b176930ce	\N	Solicitante no disponible para continuar	t	2026-07-13 14:32:54.650882+00	2026-07-13 14:32:54.650882+00	\N	\N	\N	\N
105b35a6-9474-4653-9ffc-bbb8d457cbbd	\N	Sin presupuesto aprobado	t	2026-07-13 14:32:54.650882+00	2026-07-13 14:32:54.650882+00	\N	\N	\N	\N
f757712d-dd42-4ba8-9ee3-9b676309ae78	\N	Resuelto por otro medio	t	2026-07-13 14:39:27.685044+00	2026-07-13 14:39:27.685044+00	\N	\N	\N	\N
5a156e1e-6c09-4831-b28a-5bb499a03c09	\N	Ticket duplicado	t	2026-07-13 14:39:27.685044+00	2026-07-13 14:39:27.685044+00	\N	\N	\N	\N
e191919a-c204-43f4-a3ab-2bbe4353b1c1	\N	Solicitud incorrecta o fuera de alcance	t	2026-07-13 14:39:27.685044+00	2026-07-13 14:39:27.685044+00	\N	\N	\N	\N
cc37f247-49ed-4aa5-8cd2-b3ae624efdaa	\N	Solicitante no disponible para continuar	t	2026-07-13 14:39:27.685044+00	2026-07-13 14:39:27.685044+00	\N	\N	\N	\N
5e74c245-a47c-445c-9afa-ce55b247a9c9	\N	Sin presupuesto aprobado	t	2026-07-13 14:39:27.685044+00	2026-07-13 14:39:27.685044+00	\N	\N	\N	\N
dff31a41-5dee-4ec4-972f-dd75bda13aa8	aaaaaaaa-0000-0000-0000-000000000001	Cancelacion 8189 Upd	t	2026-07-13 18:40:56.183301+00	2026-07-13 18:41:16.606181+00	cccccccc-0000-0000-0000-000000000001	cccccccc-0000-0000-0000-000000000001	\N	\N
\.


ALTER TABLE public.motivos_cancelacion ENABLE TRIGGER ALL;

--
-- Data for Name: motivos_rechazo; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.motivos_rechazo DISABLE TRIGGER ALL;

COPY public.motivos_rechazo (id, empresa_id, codigo, nombre, descripcion, es_otro, orden, activo, created_at, updated_at, created_by, updated_by, deleted_at, deleted_by) FROM stdin;
604261f8-ae9b-4c9f-b6d7-bccb97cf6732	\N	NO_RESUELTO	Problema no resuelto	La solución presentada no resuelve el problema original reportado.	f	1	t	2026-07-13 14:39:27.830504+00	2026-07-13 14:39:27.830504+00	\N	\N	\N	\N
d6ab1693-9f5a-4f6b-a9ca-4ebdc02828ea	\N	SOLUCION_INCOMPLETA	Solución incompleta	La solución es parcial y quedan aspectos pendientes sin atender.	f	2	t	2026-07-13 14:39:27.830504+00	2026-07-13 14:39:27.830504+00	\N	\N	\N	\N
1cd8d3bb-a53d-40e1-a82e-5ceb2fbe37d7	\N	PROBLEMA_PERSISTE	Problema persiste	El problema continúa ocurriendo después de aplicada la solución.	f	3	t	2026-07-13 14:39:27.830504+00	2026-07-13 14:39:27.830504+00	\N	\N	\N	\N
a411f207-e8b0-4f70-879c-cc458c169fbb	\N	ENTREGABLE_INCORRECTO	Entregable incorrecto	El entregable o resultado no corresponde a lo solicitado en el ticket.	f	4	t	2026-07-13 14:39:27.830504+00	2026-07-13 14:39:27.830504+00	\N	\N	\N	\N
8938131d-a626-44d2-9bdf-e8f5142966a5	\N	OTRO	Otro motivo	Otro motivo no contemplado en las opciones anteriores. Requiere descripción obligatoria.	t	5	t	2026-07-13 14:39:27.830504+00	2026-07-13 14:39:27.830504+00	\N	\N	\N	\N
b217730e-e157-4d37-91e4-9c6ea11f2429	aaaaaaaa-0000-0000-0000-000000000001	MR5013U	Motivo 5013 Upd	Actualizado	f	2	t	2026-07-13 18:40:19.146111+00	2026-07-13 18:40:43.577062+00	cccccccc-0000-0000-0000-000000000001	cccccccc-0000-0000-0000-000000000001	\N	\N
\.


ALTER TABLE public.motivos_rechazo ENABLE TRIGGER ALL;

--
-- Data for Name: tipos_servicio; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.tipos_servicio DISABLE TRIGGER ALL;

COPY public.tipos_servicio (id, empresa_id, nombre, descripcion, orden, activo, created_at, updated_at, created_by, updated_by, deleted_at, deleted_by) FROM stdin;
e6978a34-0c76-4169-a82c-9348bb6ba9b5	1a3ef008-e768-43f3-9bbd-0be675710cf6	Soporte de Software	Problemas con aplicaciones, sistemas internos, licencias, configuraciones o errores de software.	2	t	2026-07-17 18:56:27.540156+00	2026-07-17 18:56:27.540156+00	21540103-61cc-4141-a3f1-11763957b648	\N	\N	\N
957a3ea8-f00b-4006-b530-7ab456c9dac0	1a3ef008-e768-43f3-9bbd-0be675710cf6	Infraestructura y Redes	Solicitudes relacionadas con internet, red local, Wi-Fi, cableado, servidores y conectividad.	3	t	2026-07-17 18:56:38.553245+00	2026-07-17 18:56:38.553245+00	21540103-61cc-4141-a3f1-11763957b648	\N	\N	\N
641ca311-3280-4faa-ac49-285822a16f02	1a3ef008-e768-43f3-9bbd-0be675710cf6	Mantenimiento de Instalaciones	Reporte de incidencias en infraestructura física como iluminación, aire acondicionado, puertas, mobiliario y servicios generales.	4	t	2026-07-17 18:56:49.667867+00	2026-07-17 18:56:49.667867+00	21540103-61cc-4141-a3f1-11763957b648	\N	\N	\N
d05fdbc7-a2d0-4a1f-b5f8-aac4f1f1227e	1a3ef008-e768-43f3-9bbd-0be675710cf6	Solicitud de Servicios Generales	Requerimientos administrativos o solicitudes internas que no corresponden a una incidencia técnica específica.	5	t	2026-07-17 18:56:58.252825+00	2026-07-17 18:56:58.252825+00	21540103-61cc-4141-a3f1-11763957b648	\N	\N	\N
1985f3ba-365f-4d6f-b601-137fc846f0f6	1a3ef008-e768-43f3-9bbd-0be675710cf6	Electrico	Incidencias y solicitudes relacionadas con el sistema eléctrico, iluminación, tomacorrientes, interruptores, tableros eléctricos y suministro de energía.	6	t	2026-07-17 19:50:28.864694+00	2026-07-17 19:50:28.864694+00	21540103-61cc-4141-a3f1-11763957b648	\N	\N	\N
08279f43-36da-491f-bb06-256f388add5a	1a3ef008-e768-43f3-9bbd-0be675710cf6	Soporte de Hardware	Incidencias relacionadas con equipos físicos como computadoras, impresoras, monitores, escáneres y periféricos.	1	f	2026-07-17 18:56:06.355569+00	2026-08-12 14:45:31.82249+00	21540103-61cc-4141-a3f1-11763957b648	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
\.


ALTER TABLE public.tipos_servicio ENABLE TRIGGER ALL;

--
-- Data for Name: sla_configuraciones; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.sla_configuraciones DISABLE TRIGGER ALL;

COPY public.sla_configuraciones (id, empresa_id, tipo_servicio_id, prioridad, horas_primera_atencion, horas_resolucion, horas_laborales_inicio, horas_laborales_fin, dias_laborales, activo, created_at, updated_at, created_by, updated_by, deleted_at, deleted_by) FROM stdin;
\.


ALTER TABLE public.sla_configuraciones ENABLE TRIGGER ALL;

--
-- Data for Name: tickets; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.tickets DISABLE TRIGGER ALL;

COPY public.tickets (id, codigo, titulo, descripcion, empresa_id, sucursal_id, area_id, tipo_servicio_id, categoria_id, prioridad_solicitante, prioridad_admin, prioridad_efectiva, estado, solicitante_id, tecnico_id, ubicacion, tiempo_estimado_min, sla_id, fecha_limite_primera_atencion, fecha_limite_resolucion, valoracion, motivo_cancelacion_id, fecha_creacion, fecha_asignacion, fecha_inicio_proceso, fecha_finalizacion_tecnico, fecha_validacion, fecha_cierre, fecha_cancelacion, version, created_at, updated_at, created_by, updated_by, deleted_at, deleted_by, correos_jefe) FROM stdin;
fb23048f-b94a-41ba-9353-f1754f02fb61	PS-000036			1a3ef008-e768-43f3-9bbd-0be675710cf6	07c96055-9c8d-4aa0-8fe6-84cb70c532d2	d3389d7e-2a3c-4c4f-8981-207fa57e8622	641ca311-3280-4faa-ac49-285822a16f02	1b567db9-9f99-4c67-86ab-4885deb216dc	ALTA	\N	ALTA	ASIGNADO	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea		\N	\N	\N	\N	\N	\N	2026-07-31 01:33:16.968415+00	2026-07-31 01:35:03.254225+00	\N	\N	\N	\N	\N	2	2026-07-31 01:33:16.968415+00	2026-07-31 01:35:03.362453+00	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	21540103-61cc-4141-a3f1-11763957b648	\N	\N	{geancarlosbarrionuevo@gmail.com}
26bb057a-9c13-436f-9d2b-1178aa1ad034	PS-000040	ticket de prueba 4	ticket de prueba 4	1a3ef008-e768-43f3-9bbd-0be675710cf6	d7459f2e-2bdc-4d31-8f7d-ca37f3732706	d746db75-695c-4ddc-aa3c-d554b9c0047f	e6978a34-0c76-4169-a82c-9348bb6ba9b5	1b567db9-9f99-4c67-86ab-4885deb216dc	MEDIA	\N	MEDIA	CANCELADO	25541c55-aafe-4714-98d1-a177b057302e	\N		\N	\N	\N	\N	\N	e8205d1b-6fc0-490c-ad65-f7d48a152fcc	2026-08-03 21:00:47.482069+00	\N	\N	\N	\N	\N	2026-08-03 21:08:17.080872+00	2	2026-08-03 21:00:47.482069+00	2026-08-03 21:08:17.171034+00	25541c55-aafe-4714-98d1-a177b057302e	25541c55-aafe-4714-98d1-a177b057302e	\N	\N	{rclaudianatalia@gmail.com}
bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	PS-000042	ticket de prueba 23	pruebas de ticket 23	1a3ef008-e768-43f3-9bbd-0be675710cf6	d7459f2e-2bdc-4d31-8f7d-ca37f3732706	d746db75-695c-4ddc-aa3c-d554b9c0047f	957a3ea8-f00b-4006-b530-7ab456c9dac0	1b567db9-9f99-4c67-86ab-4885deb216dc	MEDIA	\N	MEDIA	PENDIENTE_VALIDACION	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	a72dcddd-405e-4d93-a819-d6f16bfc5f1c		\N	\N	\N	\N	\N	\N	2026-08-04 17:07:08.407164+00	2026-08-04 17:07:46.12094+00	2026-08-04 17:16:48.215331+00	2026-08-04 17:17:16.70771+00	\N	\N	\N	6	2026-08-04 17:07:08.407164+00	2026-08-04 17:17:16.742622+00	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N	{claromeroa@uch.pe}
c4a88eb1-027c-407d-b024-e9ad07a17bd1	PS-000038	nuevo ticket de prueba8	nuevo ticket de prueba nueva 2	1a3ef008-e768-43f3-9bbd-0be675710cf6	d7459f2e-2bdc-4d31-8f7d-ca37f3732706	d746db75-695c-4ddc-aa3c-d554b9c0047f	641ca311-3280-4faa-ac49-285822a16f02	1b567db9-9f99-4c67-86ab-4885deb216dc	MEDIA	\N	MEDIA	ASIGNADO	25541c55-aafe-4714-98d1-a177b057302e	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea		\N	\N	\N	\N	\N	\N	2026-08-03 16:15:59.526058+00	2026-08-06 19:50:36.905514+00	2026-08-03 16:20:59.423849+00	\N	\N	\N	\N	7	2026-08-03 16:15:59.526058+00	2026-08-06 19:50:37.017662+00	25541c55-aafe-4714-98d1-a177b057302e	21540103-61cc-4141-a3f1-11763957b648	\N	\N	{rclaudianatalia@gmail.com}
9b008df6-460a-4b34-b6cb-1d1ed878f6ab	PS-000044	ticket de prueba 23	prueba ticket 35	1a3ef008-e768-43f3-9bbd-0be675710cf6	1330cb6d-2785-488e-8710-adcb6d2fceb7	310845d3-3780-4fff-ba04-c862e5fefaee	e6978a34-0c76-4169-a82c-9348bb6ba9b5	1b567db9-9f99-4c67-86ab-4885deb216dc	ALTA	\N	ALTA	REABIERTO	ffced565-d714-42ce-a4f1-995c9511441c	\N		\N	\N	\N	\N	\N	\N	2026-08-12 14:26:54.956008+00	2026-08-18 19:51:16.713733+00	2026-08-17 14:54:38.047948+00	\N	\N	\N	\N	13	2026-08-12 14:26:54.956008+00	2026-08-18 19:55:36.008813+00	ffced565-d714-42ce-a4f1-995c9511441c	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N	{rclaudianatalia@gmail.com}
e3842e17-5cfc-4cbb-a504-010a3293a55a	PS-000039	tickedt de prueba de usuario	ticket de prueba de usuario 1	1a3ef008-e768-43f3-9bbd-0be675710cf6	d7459f2e-2bdc-4d31-8f7d-ca37f3732706	d746db75-695c-4ddc-aa3c-d554b9c0047f	e6978a34-0c76-4169-a82c-9348bb6ba9b5	1b567db9-9f99-4c67-86ab-4885deb216dc	MEDIA	\N	MEDIA	CANCELADO	25541c55-aafe-4714-98d1-a177b057302e	\N		\N	\N	\N	\N	\N	e8205d1b-6fc0-490c-ad65-f7d48a152fcc	2026-08-03 20:50:27.57354+00	\N	\N	\N	\N	\N	2026-08-03 20:52:40.691652+00	2	2026-08-03 20:50:27.57354+00	2026-08-03 20:52:40.779816+00	25541c55-aafe-4714-98d1-a177b057302e	25541c55-aafe-4714-98d1-a177b057302e	\N	\N	{rclaudianatalia@gmail.com}
6d09f602-356f-457c-ba23-e65000d73c22	PS-000029	Foco quemado en la recepción principal	Se reporta que uno de los focos ubicados en la recepción principal dejó de funcionar desde esta mañana.\n\nLa iluminación del área quedó reducida, lo que afecta la atención a los visitantes y al personal de recepción.\n\nSe verificó que el interruptor funciona correctamente y se intentó reiniciar el circuito eléctrico sin obtener resultados.\n\nSe solicita la revisión y reemplazo del foco a la brevedad.\n\nPersonas afectadas:\n- Personal de recepción.\n- Visitantes que ingresan a la sede.\n\nNo representa un riesgo inmediato, pero es recomendable solucionarlo para mantener una adecuada iluminación del área.	1a3ef008-e768-43f3-9bbd-0be675710cf6	1330cb6d-2785-488e-8710-adcb6d2fceb7	a404b2cb-7f5d-4d9e-9b38-1f9e46583755	641ca311-3280-4faa-ac49-285822a16f02	10c2eeb5-e1b3-4d28-9efb-7eb07be2b8e8	MEDIA	\N	MEDIA	SIN_ASIGNAR	21540103-61cc-4141-a3f1-11763957b648	\N	Primer piso - Recepción principal	\N	\N	\N	\N	\N	\N	2026-07-17 19:03:50.443877+00	\N	\N	\N	\N	\N	\N	1	2026-07-17 19:03:50.443877+00	2026-07-17 19:03:50.444509+00	21540103-61cc-4141-a3f1-11763957b648	21540103-61cc-4141-a3f1-11763957b648	\N	\N	{}
487f9484-9b26-42b8-8439-395d9625fe19	PS-000037	soporte tecnico prueba	se necesita soporte tecnico prueba	1a3ef008-e768-43f3-9bbd-0be675710cf6	1330cb6d-2785-488e-8710-adcb6d2fceb7	030704f2-ec6b-4ca8-953f-e0985a7b2d6c	e6978a34-0c76-4169-a82c-9348bb6ba9b5	1b567db9-9f99-4c67-86ab-4885deb216dc	MEDIA	\N	MEDIA	CANCELADO	ffced565-d714-42ce-a4f1-995c9511441c	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea		\N	\N	\N	\N	\N	e8205d1b-6fc0-490c-ad65-f7d48a152fcc	2026-08-03 14:36:27.302699+00	2026-08-03 14:53:35.095727+00	2026-08-03 15:48:52.689466+00	\N	\N	\N	2026-08-04 01:17:18.634292+00	4	2026-08-03 14:36:27.302699+00	2026-08-04 01:17:18.817913+00	ffced565-d714-42ce-a4f1-995c9511441c	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N	{rclaudianatalia@gmail.com}
8ba5f1be-4ce8-434d-be41-3f6edcc59e4f	PS-000034	asd	dasd	1a3ef008-e768-43f3-9bbd-0be675710cf6	1330cb6d-2785-488e-8710-adcb6d2fceb7	7d36b302-971b-4358-bb1c-fd902b9d0a9d	641ca311-3280-4faa-ac49-285822a16f02	1b567db9-9f99-4c67-86ab-4885deb216dc	MEDIA	\N	MEDIA	CANCELADO	21540103-61cc-4141-a3f1-11763957b648	\N		\N	\N	\N	\N	\N	e8205d1b-6fc0-490c-ad65-f7d48a152fcc	2026-07-20 22:02:03.743001+00	\N	\N	\N	\N	\N	2026-08-04 01:18:04.619337+00	3	2026-07-20 22:02:03.743001+00	2026-08-04 01:18:04.812915+00	21540103-61cc-4141-a3f1-11763957b648	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N	{}
496824d7-63ad-427b-b648-3357ccd20b83	PS-000030	error de tableros	en el area de tableros ah ocurrido errores de electricidad	1a3ef008-e768-43f3-9bbd-0be675710cf6	1330cb6d-2785-488e-8710-adcb6d2fceb7	eb390d96-3a23-4e61-80da-de3d0b7552e1	1985f3ba-365f-4d6f-b601-137fc846f0f6	1b567db9-9f99-4c67-86ab-4885deb216dc	ALTA	CRITICA	CRITICA	SIN_ASIGNAR	21540103-61cc-4141-a3f1-11763957b648	\N	Primer piso Area de tableros electricos	\N	\N	\N	\N	\N	\N	2026-07-17 20:08:36.56864+00	\N	\N	\N	\N	\N	\N	3	2026-07-17 20:08:36.56864+00	2026-07-17 20:13:26.468414+00	21540103-61cc-4141-a3f1-11763957b648	21540103-61cc-4141-a3f1-11763957b648	\N	\N	{}
e44d4b6c-f9de-4b10-926d-8eb418d445a5	PS-000031	error de el ticket	se genero un error en la creacion y actualizacion de un ticket	1a3ef008-e768-43f3-9bbd-0be675710cf6	1330cb6d-2785-488e-8710-adcb6d2fceb7	2ac21108-ff6b-4d9d-aaeb-be26a32235d3	641ca311-3280-4faa-ac49-285822a16f02	1b567db9-9f99-4c67-86ab-4885deb216dc	MEDIA	\N	MEDIA	EN_PROCESO	02419c75-3006-4f51-8019-a435201f52ba	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	planta alta - piso 7	\N	\N	\N	\N	\N	\N	2026-07-17 21:10:21.994164+00	2026-07-17 21:13:09.712223+00	2026-07-17 21:15:15.735496+00	\N	\N	\N	\N	3	2026-07-17 21:10:21.994164+00	2026-07-17 21:15:15.811449+00	02419c75-3006-4f51-8019-a435201f52ba	21540103-61cc-4141-a3f1-11763957b648	\N	\N	{}
e6bc53e4-6fd0-4965-b7c5-bcf79185bb05	PS-000033	ads	asd	1a3ef008-e768-43f3-9bbd-0be675710cf6	1330cb6d-2785-488e-8710-adcb6d2fceb7	f4897940-e51a-444d-b636-8f0143095e5b	641ca311-3280-4faa-ac49-285822a16f02	1b567db9-9f99-4c67-86ab-4885deb216dc	MEDIA	\N	MEDIA	SIN_ASIGNAR	21540103-61cc-4141-a3f1-11763957b648	\N		\N	\N	\N	\N	\N	\N	2026-07-20 20:32:41.059832+00	\N	\N	\N	\N	\N	\N	1	2026-07-20 20:32:41.059832+00	2026-07-20 20:32:41.060755+00	21540103-61cc-4141-a3f1-11763957b648	21540103-61cc-4141-a3f1-11763957b648	\N	\N	{}
d9cb455e-d34a-4ef1-bf19-14068044a85a	PS-000041	ticket de prueba de usuario 1	ticket de prueba e soporte de mantenimiento de instalaaciones.	1a3ef008-e768-43f3-9bbd-0be675710cf6	1330cb6d-2785-488e-8710-adcb6d2fceb7	310845d3-3780-4fff-ba04-c862e5fefaee	641ca311-3280-4faa-ac49-285822a16f02	1b567db9-9f99-4c67-86ab-4885deb216dc	ALTA	\N	ALTA	PENDIENTE_VALIDACION	ffced565-d714-42ce-a4f1-995c9511441c	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea		\N	\N	\N	\N	\N	\N	2026-08-04 16:04:26.876483+00	2026-08-04 16:09:19.285888+00	2026-08-04 16:10:07.284329+00	2026-08-04 16:16:47.574087+00	\N	\N	\N	6	2026-08-04 16:04:26.876483+00	2026-08-04 16:16:47.598047+00	ffced565-d714-42ce-a4f1-995c9511441c	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N	{rclaudianatalia@gmail.com}
619711dc-e907-4b64-bf75-d64e657a2790	PS-000035			1a3ef008-e768-43f3-9bbd-0be675710cf6	1330cb6d-2785-488e-8710-adcb6d2fceb7	7d36b302-971b-4358-bb1c-fd902b9d0a9d	d05fdbc7-a2d0-4a1f-b5f8-aac4f1f1227e	1b567db9-9f99-4c67-86ab-4885deb216dc	ALTA	\N	ALTA	EN_PROCESO	ffced565-d714-42ce-a4f1-995c9511441c	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea		\N	\N	\N	\N	\N	\N	2026-07-21 14:29:48.687105+00	2026-07-21 14:30:50.078511+00	2026-07-27 14:54:39.698265+00	\N	\N	\N	\N	3	2026-07-21 14:29:48.687105+00	2026-07-27 14:54:39.889786+00	ffced565-d714-42ce-a4f1-995c9511441c	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N	{}
62b633a8-4a8b-4bca-9d00-8f098c6b2452	PS-000032	prueba de titulo	porueba de descripcion completa	1a3ef008-e768-43f3-9bbd-0be675710cf6	1330cb6d-2785-488e-8710-adcb6d2fceb7	7d36b302-971b-4358-bb1c-fd902b9d0a9d	957a3ea8-f00b-4006-b530-7ab456c9dac0	1b567db9-9f99-4c67-86ab-4885deb216dc	ALTA	\N	ALTA	ASIGNADO	21540103-61cc-4141-a3f1-11763957b648	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea		\N	\N	\N	\N	\N	\N	2026-07-18 18:14:23.850968+00	2026-08-06 19:51:26.51937+00	\N	\N	\N	\N	\N	2	2026-07-18 18:14:23.850968+00	2026-08-06 19:51:26.548743+00	21540103-61cc-4141-a3f1-11763957b648	21540103-61cc-4141-a3f1-11763957b648	\N	\N	{}
f856d580-0907-4f8f-ae86-7007cf8d527b	PS-000043	ticket de prueba	ticket de prueba , filtros	1a3ef008-e768-43f3-9bbd-0be675710cf6	1330cb6d-2785-488e-8710-adcb6d2fceb7	310845d3-3780-4fff-ba04-c862e5fefaee	957a3ea8-f00b-4006-b530-7ab456c9dac0	1b567db9-9f99-4c67-86ab-4885deb216dc	MEDIA	\N	MEDIA	PENDIENTE_VALIDACION	ffced565-d714-42ce-a4f1-995c9511441c	a72dcddd-405e-4d93-a819-d6f16bfc5f1c		\N	\N	\N	\N	\N	\N	2026-08-12 13:42:10.036683+00	2026-08-12 13:44:03.545774+00	2026-08-12 14:03:38.620798+00	2026-08-12 14:06:14.329681+00	\N	\N	\N	4	2026-08-12 13:42:10.036683+00	2026-08-12 14:06:14.417294+00	ffced565-d714-42ce-a4f1-995c9511441c	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N	{rclaudianatalia@gmail.com}
f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	PS-000045	tickte de pruebas 18 de agosto	creacion de prueba de tickets.	1a3ef008-e768-43f3-9bbd-0be675710cf6	a48d3ab2-9057-45a4-980b-ce28ef27f028	49e16dd4-ac01-42eb-beb8-46b84e40a987	e6978a34-0c76-4169-a82c-9348bb6ba9b5	1b567db9-9f99-4c67-86ab-4885deb216dc	MEDIA	\N	MEDIA	CERRADO	ffced565-d714-42ce-a4f1-995c9511441c	a72dcddd-405e-4d93-a819-d6f16bfc5f1c		\N	\N	\N	\N	\N	\N	2026-08-18 17:42:55.528933+00	2026-08-18 17:51:20.352897+00	2026-08-18 19:37:24.200258+00	2026-08-18 19:38:43.816265+00	2026-08-18 19:45:58.835733+00	2026-08-18 19:45:58.835733+00	\N	5	2026-08-18 17:42:55.528933+00	2026-08-18 19:45:58.926919+00	ffced565-d714-42ce-a4f1-995c9511441c	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N	{claromeroa@uch.pe}
\.


ALTER TABLE public.tickets ENABLE TRIGGER ALL;

--
-- Data for Name: notificaciones; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.notificaciones DISABLE TRIGGER ALL;

COPY public.notificaciones (id, destinatario_id, tipo_evento, titulo, cuerpo, prioridad, canal, estado_entrega, leida, leida_en, ticket_id, metadata, created_at, updated_at, created_by, updated_by) FROM stdin;
8b70a628-b26c-4266-8b10-e4db5eb1f772	21540103-61cc-4141-a3f1-11763957b648	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000029: Foco quemado en la recepción principal	medium	IN_APP	PENDIENTE	t	2026-07-17 19:41:38.537988+00	6d09f602-356f-457c-ba23-e65000d73c22	\N	2026-07-17 19:03:51.544023+00	2026-07-17 19:41:38.646032+00	\N	\N
9d7355d9-41b7-47f2-aa8c-f17569dbc7cc	21540103-61cc-4141-a3f1-11763957b648	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000030: error de tableros	medium	IN_APP	PENDIENTE	t	2026-07-17 20:19:46.768294+00	496824d7-63ad-427b-b648-3357ccd20b83	\N	2026-07-17 20:08:37.937688+00	2026-07-17 20:19:46.840493+00	\N	\N
1ce8a33d-53e4-44b0-bd67-0bce0bb705e4	21540103-61cc-4141-a3f1-11763957b648	ticket.asignado	Ticket asignado	El ticket PS-000031 fue asignado: error de el ticket	medium	IN_APP	PENDIENTE	t	2026-07-17 21:13:28.206788+00	e44d4b6c-f9de-4b10-926d-8eb418d445a5	\N	2026-07-17 21:13:10.495218+00	2026-07-17 21:13:28.295211+00	\N	\N
22dcfe46-1823-4444-a87c-cb808f6f6bab	02419c75-3006-4f51-8019-a435201f52ba	ticket.en_proceso	Ticket en proceso	El técnico ha iniciado la atención del ticket PS-000031: error de el ticket	medium	IN_APP	PENDIENTE	f	\N	e44d4b6c-f9de-4b10-926d-8eb418d445a5	\N	2026-07-17 21:15:16.192968+00	2026-07-17 21:15:16.192968+00	\N	\N
ee5128af-0e87-438b-a592-6f3fab9780f6	ffced565-d714-42ce-a4f1-995c9511441c	ticket.asignado	Ticket asignado	El ticket PS-000035 fue asignado:	medium	IN_APP	PENDIENTE	f	\N	619711dc-e907-4b64-bf75-d64e657a2790	\N	2026-07-21 14:30:50.869833+00	2026-07-21 14:30:50.869833+00	\N	\N
f31a30ef-e49f-4391-a2e5-d455cdc8688a	ffced565-d714-42ce-a4f1-995c9511441c	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000035:	medium	IN_APP	PENDIENTE	t	2026-07-21 14:31:10.77557+00	619711dc-e907-4b64-bf75-d64e657a2790	\N	2026-07-21 14:29:49.677817+00	2026-07-21 14:31:11.170411+00	\N	\N
0c4ac780-d393-45ee-ac75-b9903078f35f	ffced565-d714-42ce-a4f1-995c9511441c	ticket.en_proceso	Ticket en proceso	El técnico ha iniciado la atención del ticket PS-000035:	medium	IN_APP	PENDIENTE	f	\N	619711dc-e907-4b64-bf75-d64e657a2790	\N	2026-07-27 14:54:40.59229+00	2026-07-27 14:54:40.59229+00	\N	\N
4f1e165b-bf1c-4a88-9285-f1b3eac37a36	ffced565-d714-42ce-a4f1-995c9511441c	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000035:	medium	IN_APP	PENDIENTE	t	2026-07-27 17:46:50.03071+00	619711dc-e907-4b64-bf75-d64e657a2790	\N	2026-07-27 14:54:41.09172+00	2026-07-27 17:46:50.42608+00	\N	\N
81efdbee-ad2b-4577-a4d2-4baf9c7078fa	ffced565-d714-42ce-a4f1-995c9511441c	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000036:	medium	IN_APP	PENDIENTE	f	\N	fb23048f-b94a-41ba-9353-f1754f02fb61	\N	2026-07-31 01:33:18.866518+00	2026-07-31 01:33:18.866518+00	\N	\N
a3878b34-33c3-4912-9364-d5dd395d05af	ffced565-d714-42ce-a4f1-995c9511441c	ticket.asignado	Ticket asignado	El ticket PS-000036 fue asignado:	medium	IN_APP	PENDIENTE	f	\N	fb23048f-b94a-41ba-9353-f1754f02fb61	\N	2026-07-31 01:35:04.052439+00	2026-07-31 01:35:04.052439+00	\N	\N
5f392849-6990-4829-ae09-09bd80ec5154	ffced565-d714-42ce-a4f1-995c9511441c	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000037: soporte tecnico prueba	medium	IN_APP	PENDIENTE	f	\N	487f9484-9b26-42b8-8439-395d9625fe19	\N	2026-08-03 14:36:29.194588+00	2026-08-03 14:36:29.194588+00	\N	\N
194f805d-5c10-4f00-9aac-87070cef5816	ffced565-d714-42ce-a4f1-995c9511441c	ticket.asignado	Ticket asignado	El ticket PS-000037 fue asignado: soporte tecnico prueba	medium	IN_APP	PENDIENTE	t	2026-08-03 15:29:09.999924+00	487f9484-9b26-42b8-8439-395d9625fe19	\N	2026-08-03 14:53:36.196461+00	2026-08-03 15:29:10.11026+00	\N	\N
d32af683-0e6c-4364-9f0a-c6c227d5459e	ffced565-d714-42ce-a4f1-995c9511441c	ticket.en_proceso	Ticket en proceso	El técnico ha iniciado la atención del ticket PS-000037: soporte tecnico prueba	medium	IN_APP	PENDIENTE	f	\N	487f9484-9b26-42b8-8439-395d9625fe19	\N	2026-08-03 15:48:53.088839+00	2026-08-03 15:48:53.088839+00	\N	\N
dad437cf-bd77-4be5-b731-5e62ee2f1daa	ffced565-d714-42ce-a4f1-995c9511441c	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000037: soporte tecnico prueba	medium	IN_APP	PENDIENTE	f	\N	487f9484-9b26-42b8-8439-395d9625fe19	\N	2026-08-03 15:48:53.310902+00	2026-08-03 15:48:53.310902+00	\N	\N
2ab25171-2333-49af-98b9-6f1a950c92aa	ffced565-d714-42ce-a4f1-995c9511441c	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000038: nuevo ticket de prueba	medium	IN_APP	PENDIENTE	f	\N	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	2026-08-03 16:16:00.98875+00	2026-08-03 16:16:00.98875+00	\N	\N
8bd532ba-eeda-4332-95f4-7f846af1ef3a	ffced565-d714-42ce-a4f1-995c9511441c	ticket.asignado	Ticket asignado	El ticket PS-000038 fue asignado: nuevo ticket de prueba	medium	IN_APP	PENDIENTE	f	\N	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	2026-08-03 16:19:14.694738+00	2026-08-03 16:19:14.694738+00	\N	\N
89273cbf-0cae-4527-814c-8a64251271b6	ffced565-d714-42ce-a4f1-995c9511441c	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000038: nuevo ticket de prueba	medium	IN_APP	PENDIENTE	f	\N	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	2026-08-03 16:21:00.088828+00	2026-08-03 16:21:00.088828+00	\N	\N
659b77e8-f279-4466-b76d-3afe40ff4d03	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	ticket.asignado	Ticket asignado	Se te ha asignado el ticket PS-000031: error de el ticket	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:11.331394+00	e44d4b6c-f9de-4b10-926d-8eb418d445a5	\N	2026-07-17 21:13:10.397259+00	2026-08-06 19:49:11.331394+00	\N	\N
82009fa7-6d9e-4b16-8491-852a3e279c44	21540103-61cc-4141-a3f1-11763957b648	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000031: error de el ticket	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	e44d4b6c-f9de-4b10-926d-8eb418d445a5	\N	2026-07-17 21:10:22.999787+00	2026-08-06 19:49:39.814183+00	\N	\N
1e72cc3b-199b-4e2d-8c99-09eb071a41cf	21540103-61cc-4141-a3f1-11763957b648	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000031: error de el ticket	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	e44d4b6c-f9de-4b10-926d-8eb418d445a5	\N	2026-07-17 21:15:16.29295+00	2026-08-06 19:49:39.814183+00	\N	\N
275967b6-dff7-4ad8-8325-46fe725d4bb5	21540103-61cc-4141-a3f1-11763957b648	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000032: prueba de titulo	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	62b633a8-4a8b-4bca-9d00-8f098c6b2452	\N	2026-07-18 18:14:25.055714+00	2026-08-06 19:49:39.814183+00	\N	\N
30be90a2-2b66-45b2-91cb-1d0ff2926b88	21540103-61cc-4141-a3f1-11763957b648	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000033: ads	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	e6bc53e4-6fd0-4965-b7c5-bcf79185bb05	\N	2026-07-20 20:32:42.43835+00	2026-08-06 19:49:39.814183+00	\N	\N
a35dd9e6-df01-4df4-a5bf-3c1e05ba92dc	21540103-61cc-4141-a3f1-11763957b648	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000034:	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	8ba5f1be-4ce8-434d-be41-3f6edcc59e4f	\N	2026-07-20 22:02:05.250234+00	2026-08-06 19:49:39.814183+00	\N	\N
ec78334f-af11-41b9-8003-d82d4e5a19d6	ffced565-d714-42ce-a4f1-995c9511441c	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000038 está listo para validación: nuevo ticket de prueba	medium	IN_APP	PENDIENTE	f	\N	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	2026-08-03 16:24:20.589057+00	2026-08-03 16:24:20.589057+00	\N	\N
aa5c2c2d-2a69-403d-a0c5-3e8ba1319327	ffced565-d714-42ce-a4f1-995c9511441c	ticket.rechazado	Ticket reabierto	El ticket PS-000038 ha sido rechazado y requiere reasignación.	medium	IN_APP	PENDIENTE	f	\N	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	2026-08-03 19:51:59.354383+00	2026-08-03 19:51:59.354383+00	\N	\N
e6bc48c8-7ea7-4b78-beef-9f1a69ff70db	ffced565-d714-42ce-a4f1-995c9511441c	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000039: tickedt de prueba de usuario	medium	IN_APP	PENDIENTE	f	\N	e3842e17-5cfc-4cbb-a504-010a3293a55a	\N	2026-08-03 20:50:29.070625+00	2026-08-03 20:50:29.070625+00	\N	\N
aac65554-205e-456a-8699-41fcc491f407	ffced565-d714-42ce-a4f1-995c9511441c	ticket.cancelado	Ticket cancelado	El ticket PS-000039 fue cancelado: tickedt de prueba de usuario	medium	IN_APP	PENDIENTE	f	\N	e3842e17-5cfc-4cbb-a504-010a3293a55a	\N	2026-08-03 20:52:41.37093+00	2026-08-03 20:52:41.37093+00	\N	\N
18746c83-7a7b-47ce-8293-0dac04b5fa74	25541c55-aafe-4714-98d1-a177b057302e	ticket.en_proceso	Ticket en proceso	El técnico ha iniciado la atención del ticket PS-000038: nuevo ticket de prueba	medium	IN_APP	PENDIENTE	t	2026-08-03 20:54:53.278398+00	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	2026-08-03 16:20:59.888753+00	2026-08-03 20:54:53.278398+00	\N	\N
1af109fc-638f-4033-ab4e-295f49a48f39	25541c55-aafe-4714-98d1-a177b057302e	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000038 está listo para tu validación: nuevo ticket de prueba	medium	IN_APP	PENDIENTE	t	2026-08-03 20:54:53.278398+00	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	2026-08-03 16:24:20.410275+00	2026-08-03 20:54:53.278398+00	\N	\N
e67dc100-8727-4c27-86d0-bc5dfb68a014	ffced565-d714-42ce-a4f1-995c9511441c	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000040: ticket de prueba 4	medium	IN_APP	PENDIENTE	f	\N	26bb057a-9c13-436f-9d2b-1178aa1ad034	\N	2026-08-03 21:00:48.069119+00	2026-08-03 21:00:48.069119+00	\N	\N
cbf044f0-d559-4f24-b274-025dc1036d76	ffced565-d714-42ce-a4f1-995c9511441c	ticket.cancelado	Ticket cancelado	El ticket PS-000040 fue cancelado: ticket de prueba 4	medium	IN_APP	PENDIENTE	t	2026-08-03 21:12:52.661123+00	26bb057a-9c13-436f-9d2b-1178aa1ad034	\N	2026-08-03 21:08:17.564938+00	2026-08-03 21:12:53.163591+00	\N	\N
2ed2676a-4c5c-46e1-95da-7d487d05920f	ffced565-d714-42ce-a4f1-995c9511441c	ticket.cancelado	Ticket cancelado	El ticket PS-000037 fue cancelado: soporte tecnico prueba	medium	IN_APP	PENDIENTE	f	\N	487f9484-9b26-42b8-8439-395d9625fe19	\N	2026-08-04 01:17:19.915795+00	2026-08-04 01:17:19.915795+00	\N	\N
797161ac-9fc2-40cc-865b-b658c284abb6	ffced565-d714-42ce-a4f1-995c9511441c	ticket.cancelado	Ticket cancelado	El ticket PS-000034 fue cancelado: asd	medium	IN_APP	PENDIENTE	f	\N	8ba5f1be-4ce8-434d-be41-3f6edcc59e4f	\N	2026-08-04 01:18:05.210668+00	2026-08-04 01:18:05.210668+00	\N	\N
658e7172-669e-4888-a00b-32302a554be8	ffced565-d714-42ce-a4f1-995c9511441c	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000041: ticket de prueba de usuario 1	medium	IN_APP	PENDIENTE	f	\N	d9cb455e-d34a-4ef1-bf19-14068044a85a	\N	2026-08-04 16:04:28.676722+00	2026-08-04 16:04:28.676722+00	\N	\N
022ee039-7731-4d62-8e92-85c57169310c	ffced565-d714-42ce-a4f1-995c9511441c	ticket.asignado	Ticket asignado	El ticket PS-000041 fue asignado: ticket de prueba de usuario 1	medium	IN_APP	PENDIENTE	f	\N	d9cb455e-d34a-4ef1-bf19-14068044a85a	\N	2026-08-04 16:09:20.193148+00	2026-08-04 16:09:20.193148+00	\N	\N
8cae64f9-0a73-46e3-8343-b1398d3401da	ffced565-d714-42ce-a4f1-995c9511441c	ticket.en_proceso	Ticket en proceso	El técnico ha iniciado la atención del ticket PS-000041: ticket de prueba de usuario 1	medium	IN_APP	PENDIENTE	f	\N	d9cb455e-d34a-4ef1-bf19-14068044a85a	\N	2026-08-04 16:10:07.670301+00	2026-08-04 16:10:07.670301+00	\N	\N
62626433-e3af-4bb7-8ee8-2f790a8e9bdb	ffced565-d714-42ce-a4f1-995c9511441c	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000041: ticket de prueba de usuario 1	medium	IN_APP	PENDIENTE	f	\N	d9cb455e-d34a-4ef1-bf19-14068044a85a	\N	2026-08-04 16:10:07.782951+00	2026-08-04 16:10:07.782951+00	\N	\N
4b4475fc-26e4-4d2e-83e0-2d22f0bfa6cd	ffced565-d714-42ce-a4f1-995c9511441c	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000041 está listo para tu validación: ticket de prueba de usuario 1	medium	IN_APP	PENDIENTE	f	\N	d9cb455e-d34a-4ef1-bf19-14068044a85a	\N	2026-08-04 16:16:47.978375+00	2026-08-04 16:16:47.978375+00	\N	\N
89a232c2-c5b4-4f24-9e2b-580ec63079a1	ffced565-d714-42ce-a4f1-995c9511441c	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000041 está listo para validación: ticket de prueba de usuario 1	medium	IN_APP	PENDIENTE	f	\N	d9cb455e-d34a-4ef1-bf19-14068044a85a	\N	2026-08-04 16:16:48.374241+00	2026-08-04 16:16:48.374241+00	\N	\N
46fb1963-9375-433e-8398-83142694c932	ffced565-d714-42ce-a4f1-995c9511441c	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000042: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	\N	2026-08-04 17:07:09.611939+00	2026-08-04 17:07:09.611939+00	\N	\N
eecbf199-2ed2-4b3c-be74-0faddd373761	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	ticket.asignado	Ticket asignado	Se te ha asignado el ticket PS-000042: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	\N	2026-08-04 17:07:46.906553+00	2026-08-04 17:07:46.906553+00	\N	\N
837cbab5-3bde-4a32-9be7-f37a6edd9c70	21540103-61cc-4141-a3f1-11763957b648	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000038: nuevo ticket de prueba	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	2026-08-03 16:21:00.089211+00	2026-08-06 19:49:39.814183+00	\N	\N
ff5881ec-872c-424d-8285-731dc3988eac	21540103-61cc-4141-a3f1-11763957b648	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000038 está listo para validación: nuevo ticket de prueba	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	2026-08-03 16:24:20.592387+00	2026-08-06 19:49:39.814183+00	\N	\N
d12a75c4-b6e5-4ce0-ba9d-cfb43c2d172a	21540103-61cc-4141-a3f1-11763957b648	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000039: tickedt de prueba de usuario	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	e3842e17-5cfc-4cbb-a504-010a3293a55a	\N	2026-08-03 20:50:29.078321+00	2026-08-06 19:49:39.814183+00	\N	\N
803996be-500f-43a2-820f-395fb14ba6d2	ffced565-d714-42ce-a4f1-995c9511441c	ticket.asignado	Ticket asignado	El ticket PS-000042 fue asignado: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	\N	2026-08-04 17:07:47.110012+00	2026-08-04 17:07:47.110012+00	\N	\N
f38dc93a-6a6a-4fd4-af92-2cdb2c4588a7	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	ticket.en_proceso	Ticket en proceso	El técnico ha iniciado la atención del ticket PS-000042: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	\N	2026-08-04 17:16:48.708697+00	2026-08-04 17:16:48.708697+00	\N	\N
984c1456-71ab-4e03-a947-d00f5114fede	ffced565-d714-42ce-a4f1-995c9511441c	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000042: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	\N	2026-08-04 17:16:48.712117+00	2026-08-04 17:16:48.712117+00	\N	\N
177ac656-c93f-4950-9159-037bd4f2ba1a	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000042 está listo para tu validación: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	\N	2026-08-04 17:17:17.41+00	2026-08-04 17:17:17.41+00	\N	\N
99d425e9-1dea-4015-9820-a06f63930161	ffced565-d714-42ce-a4f1-995c9511441c	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000042 está listo para validación: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	\N	2026-08-04 17:17:17.531506+00	2026-08-04 17:17:17.531506+00	\N	\N
268a9b39-8b89-40b3-a6b8-6a5c41335bf1	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	ticket.asignado	Ticket asignado	Se te ha asignado el ticket PS-000035:	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:11.331394+00	619711dc-e907-4b64-bf75-d64e657a2790	\N	2026-07-21 14:30:50.66914+00	2026-08-06 19:49:11.331394+00	\N	\N
4c6afaa8-5597-400d-93a0-26fd325d9f3e	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	ticket.asignado	Ticket asignado	Se te ha asignado el ticket PS-000036:	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:11.331394+00	fb23048f-b94a-41ba-9353-f1754f02fb61	\N	2026-07-31 01:35:03.86323+00	2026-08-06 19:49:11.331394+00	\N	\N
403bb43f-34c3-4ee8-8858-ccf0c3ea7e47	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	ticket.asignado	Ticket asignado	Se te ha asignado el ticket PS-000037: soporte tecnico prueba	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:11.331394+00	487f9484-9b26-42b8-8439-395d9625fe19	\N	2026-08-03 14:53:35.794861+00	2026-08-06 19:49:11.331394+00	\N	\N
32e79dc6-1ecf-4ff7-8823-e7e7dc23cf28	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	ticket.asignado	Ticket asignado	Se te ha asignado el ticket PS-000038: nuevo ticket de prueba	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:11.331394+00	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	2026-08-03 16:19:14.593927+00	2026-08-06 19:49:11.331394+00	\N	\N
1e0f5837-8c4a-42bd-838c-c3730401801e	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	ticket.rechazado	Ticket rechazado	El ticket PS-000038 ha sido rechazado y está pendiente de reasignación.	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:11.331394+00	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	2026-08-03 19:51:58.471483+00	2026-08-06 19:49:11.331394+00	\N	\N
e9028440-e739-4db8-a4c2-a08909b0d543	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	ticket.cancelado	Ticket cancelado	El ticket PS-000037 ha sido cancelado: soporte tecnico prueba	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:11.331394+00	487f9484-9b26-42b8-8439-395d9625fe19	\N	2026-08-04 01:17:19.512659+00	2026-08-06 19:49:11.331394+00	\N	\N
c9faf0b9-0a5c-420d-ae4e-707181f343c4	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	ticket.asignado	Ticket asignado	Se te ha asignado el ticket PS-000041: ticket de prueba de usuario 1	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:11.331394+00	d9cb455e-d34a-4ef1-bf19-14068044a85a	\N	2026-08-04 16:09:19.981878+00	2026-08-06 19:49:11.331394+00	\N	\N
93081876-e381-4229-a520-fec70834180b	21540103-61cc-4141-a3f1-11763957b648	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000035:	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	619711dc-e907-4b64-bf75-d64e657a2790	\N	2026-07-21 14:29:49.679642+00	2026-08-06 19:49:39.814183+00	\N	\N
a6cb9342-fdc0-4fa7-86ff-4357cd516299	21540103-61cc-4141-a3f1-11763957b648	ticket.asignado	Ticket asignado	El ticket PS-000035 fue asignado:	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	619711dc-e907-4b64-bf75-d64e657a2790	\N	2026-07-21 14:30:50.870012+00	2026-08-06 19:49:39.814183+00	\N	\N
26e937a6-3740-4528-b5a5-45f4aea72c73	21540103-61cc-4141-a3f1-11763957b648	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000035:	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	619711dc-e907-4b64-bf75-d64e657a2790	\N	2026-07-27 14:54:41.187595+00	2026-08-06 19:49:39.814183+00	\N	\N
f9c15bb5-db5f-459f-b1f5-39bd5a472f39	21540103-61cc-4141-a3f1-11763957b648	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000036:	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	fb23048f-b94a-41ba-9353-f1754f02fb61	\N	2026-07-31 01:33:18.962496+00	2026-08-06 19:49:39.814183+00	\N	\N
0cbbd78e-93e5-45a2-9211-7c27867cf884	21540103-61cc-4141-a3f1-11763957b648	ticket.asignado	Ticket asignado	El ticket PS-000036 fue asignado:	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	fb23048f-b94a-41ba-9353-f1754f02fb61	\N	2026-07-31 01:35:04.052233+00	2026-08-06 19:49:39.814183+00	\N	\N
36c411fb-b759-4038-9dd6-34c6df4ef161	21540103-61cc-4141-a3f1-11763957b648	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000037: soporte tecnico prueba	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	487f9484-9b26-42b8-8439-395d9625fe19	\N	2026-08-03 14:36:29.196858+00	2026-08-06 19:49:39.814183+00	\N	\N
83d8f145-c1d9-4fbd-a20f-f8f3f28e27ab	21540103-61cc-4141-a3f1-11763957b648	ticket.asignado	Ticket asignado	El ticket PS-000037 fue asignado: soporte tecnico prueba	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	487f9484-9b26-42b8-8439-395d9625fe19	\N	2026-08-03 14:53:36.191972+00	2026-08-06 19:49:39.814183+00	\N	\N
e652ce08-ec79-4e2c-9c66-24b164e76132	21540103-61cc-4141-a3f1-11763957b648	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000037: soporte tecnico prueba	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	487f9484-9b26-42b8-8439-395d9625fe19	\N	2026-08-03 15:48:53.313292+00	2026-08-06 19:49:39.814183+00	\N	\N
58640971-b521-402a-9e4f-1d1f519f13bf	21540103-61cc-4141-a3f1-11763957b648	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000038: nuevo ticket de prueba	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	2026-08-03 16:16:00.98875+00	2026-08-06 19:49:39.814183+00	\N	\N
c5fa6b4c-e66d-445a-9925-ced6d84f9144	21540103-61cc-4141-a3f1-11763957b648	ticket.asignado	Ticket asignado	El ticket PS-000038 fue asignado: nuevo ticket de prueba	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	2026-08-03 16:19:14.697638+00	2026-08-06 19:49:39.814183+00	\N	\N
606a9518-1185-4781-9cfc-89cc038c32a2	21540103-61cc-4141-a3f1-11763957b648	ticket.rechazado	Ticket reabierto	El ticket PS-000038 ha sido rechazado y requiere reasignación.	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	2026-08-03 19:51:59.453346+00	2026-08-06 19:49:39.814183+00	\N	\N
a8f22ef3-bca1-4c27-b258-0d3f384f6cc0	21540103-61cc-4141-a3f1-11763957b648	ticket.cancelado	Ticket cancelado	El ticket PS-000039 fue cancelado: tickedt de prueba de usuario	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	e3842e17-5cfc-4cbb-a504-010a3293a55a	\N	2026-08-03 20:52:41.37093+00	2026-08-06 19:49:39.814183+00	\N	\N
5fb73783-f1a2-4f62-8a74-7ada6fcf06c6	21540103-61cc-4141-a3f1-11763957b648	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000040: ticket de prueba 4	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	26bb057a-9c13-436f-9d2b-1178aa1ad034	\N	2026-08-03 21:00:48.080418+00	2026-08-06 19:49:39.814183+00	\N	\N
314b0052-0b79-4dae-92d6-6a046aa9051a	21540103-61cc-4141-a3f1-11763957b648	ticket.cancelado	Ticket cancelado	El ticket PS-000040 fue cancelado: ticket de prueba 4	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	26bb057a-9c13-436f-9d2b-1178aa1ad034	\N	2026-08-03 21:08:17.566686+00	2026-08-06 19:49:39.814183+00	\N	\N
a5bb404c-62a9-4842-9ae8-11e8256ca97c	21540103-61cc-4141-a3f1-11763957b648	ticket.cancelado	Ticket cancelado	El ticket PS-000037 fue cancelado: soporte tecnico prueba	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	487f9484-9b26-42b8-8439-395d9625fe19	\N	2026-08-04 01:17:19.914566+00	2026-08-06 19:49:39.814183+00	\N	\N
8944bb16-ef3c-4b65-a6fb-bfa6c8475e17	21540103-61cc-4141-a3f1-11763957b648	ticket.cancelado	Ticket cancelado	Tu ticket PS-000034 ha sido cancelado: asd	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	8ba5f1be-4ce8-434d-be41-3f6edcc59e4f	\N	2026-08-04 01:18:05.025559+00	2026-08-06 19:49:39.814183+00	\N	\N
c5af880b-f8cf-4bda-b7e0-b5d475a895a4	21540103-61cc-4141-a3f1-11763957b648	ticket.cancelado	Ticket cancelado	El ticket PS-000034 fue cancelado: asd	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	8ba5f1be-4ce8-434d-be41-3f6edcc59e4f	\N	2026-08-04 01:18:05.210923+00	2026-08-06 19:49:39.814183+00	\N	\N
fc65d33b-d8da-47ca-96e4-ced81c5c521c	21540103-61cc-4141-a3f1-11763957b648	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000041: ticket de prueba de usuario 1	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	d9cb455e-d34a-4ef1-bf19-14068044a85a	\N	2026-08-04 16:04:28.79087+00	2026-08-06 19:49:39.814183+00	\N	\N
da6d484f-0ce4-4995-9809-60ca45e87b42	21540103-61cc-4141-a3f1-11763957b648	ticket.asignado	Ticket asignado	El ticket PS-000041 fue asignado: ticket de prueba de usuario 1	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	d9cb455e-d34a-4ef1-bf19-14068044a85a	\N	2026-08-04 16:09:20.188651+00	2026-08-06 19:49:39.814183+00	\N	\N
1b38b1b4-6d29-457d-89a8-2cf8c8bf0845	21540103-61cc-4141-a3f1-11763957b648	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000041: ticket de prueba de usuario 1	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	d9cb455e-d34a-4ef1-bf19-14068044a85a	\N	2026-08-04 16:10:07.787776+00	2026-08-06 19:49:39.814183+00	\N	\N
87175552-32a2-4b73-9991-675532f7f8f9	21540103-61cc-4141-a3f1-11763957b648	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000041 está listo para validación: ticket de prueba de usuario 1	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	d9cb455e-d34a-4ef1-bf19-14068044a85a	\N	2026-08-04 16:16:48.374273+00	2026-08-06 19:49:39.814183+00	\N	\N
0ff60555-5c03-470c-b155-87718b00f49b	21540103-61cc-4141-a3f1-11763957b648	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000042: ticket de prueba 23	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	\N	2026-08-04 17:07:09.617515+00	2026-08-06 19:49:39.814183+00	\N	\N
d0e26f6e-f733-45b3-9f03-4631244df456	21540103-61cc-4141-a3f1-11763957b648	ticket.asignado	Ticket asignado	El ticket PS-000042 fue asignado: ticket de prueba 23	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	\N	2026-08-04 17:07:47.114888+00	2026-08-06 19:49:39.814183+00	\N	\N
627da785-1b36-488b-ab24-a96c95993b27	21540103-61cc-4141-a3f1-11763957b648	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000042: ticket de prueba 23	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	\N	2026-08-04 17:16:48.716171+00	2026-08-06 19:49:39.814183+00	\N	\N
85a06626-8745-4b9c-8538-18cb99e79734	21540103-61cc-4141-a3f1-11763957b648	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000042 está listo para validación: ticket de prueba 23	medium	IN_APP	PENDIENTE	t	2026-08-06 19:49:39.814183+00	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	\N	2026-08-04 17:17:17.712782+00	2026-08-06 19:49:39.814183+00	\N	\N
4011779f-d74a-4945-a3d0-edc9988e834c	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	ticket.asignado	Ticket asignado	Se te ha asignado el ticket PS-000038: nuevo ticket de prueba8	medium	IN_APP	PENDIENTE	f	\N	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	2026-08-06 19:50:37.632995+00	2026-08-06 19:50:37.632995+00	\N	\N
d5d822b9-a390-4db7-89ad-4cf7c9a354a2	ffced565-d714-42ce-a4f1-995c9511441c	ticket.asignado	Ticket asignado	El ticket PS-000038 fue asignado: nuevo ticket de prueba8	medium	IN_APP	PENDIENTE	f	\N	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	2026-08-06 19:50:37.814577+00	2026-08-06 19:50:37.814577+00	\N	\N
2d7cd6ee-003d-4f73-9950-82b0931ed6ac	21540103-61cc-4141-a3f1-11763957b648	ticket.asignado	Ticket asignado	El ticket PS-000038 fue asignado: nuevo ticket de prueba8	medium	IN_APP	PENDIENTE	f	\N	c4a88eb1-027c-407d-b024-e9ad07a17bd1	\N	2026-08-06 19:50:37.815131+00	2026-08-06 19:50:37.815131+00	\N	\N
50879db6-2f69-4c96-b1d7-ebacf27cb903	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	ticket.asignado	Ticket asignado	Se te ha asignado el ticket PS-000032: prueba de titulo	medium	IN_APP	PENDIENTE	f	\N	62b633a8-4a8b-4bca-9d00-8f098c6b2452	\N	2026-08-06 19:51:26.93869+00	2026-08-06 19:51:26.93869+00	\N	\N
e50b72a3-cef6-463a-a47e-81b640287abf	21540103-61cc-4141-a3f1-11763957b648	ticket.asignado	Ticket asignado	El ticket PS-000032 fue asignado: prueba de titulo	medium	IN_APP	PENDIENTE	f	\N	62b633a8-4a8b-4bca-9d00-8f098c6b2452	\N	2026-08-06 19:51:27.118215+00	2026-08-06 19:51:27.118215+00	\N	\N
5684903f-66ec-404d-b682-aa7a216bde1a	ffced565-d714-42ce-a4f1-995c9511441c	ticket.asignado	Ticket asignado	El ticket PS-000032 fue asignado: prueba de titulo	medium	IN_APP	PENDIENTE	f	\N	62b633a8-4a8b-4bca-9d00-8f098c6b2452	\N	2026-08-06 19:51:27.117799+00	2026-08-06 19:51:27.117799+00	\N	\N
13bce8c1-5c48-43f6-8ef9-37770288c24b	ffced565-d714-42ce-a4f1-995c9511441c	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000043: ticket de prueba	medium	IN_APP	PENDIENTE	f	\N	f856d580-0907-4f8f-ae86-7007cf8d527b	\N	2026-08-12 13:42:12.010138+00	2026-08-12 13:42:12.010138+00	\N	\N
9e06a540-d5b8-4751-b6c4-a49b06c85df1	21540103-61cc-4141-a3f1-11763957b648	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000043: ticket de prueba	medium	IN_APP	PENDIENTE	f	\N	f856d580-0907-4f8f-ae86-7007cf8d527b	\N	2026-08-12 13:42:12.017747+00	2026-08-12 13:42:12.017747+00	\N	\N
4c728be1-f52f-442d-98eb-d3f44509b263	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	ticket.asignado	Ticket asignado	Se te ha asignado el ticket PS-000043: ticket de prueba	medium	IN_APP	PENDIENTE	f	\N	f856d580-0907-4f8f-ae86-7007cf8d527b	\N	2026-08-12 13:44:04.212447+00	2026-08-12 13:44:04.212447+00	\N	\N
9e280cf0-888e-44ef-9b2a-91ad85e98f80	21540103-61cc-4141-a3f1-11763957b648	ticket.asignado	Ticket asignado	El ticket PS-000043 fue asignado: ticket de prueba	medium	IN_APP	PENDIENTE	f	\N	f856d580-0907-4f8f-ae86-7007cf8d527b	\N	2026-08-12 13:44:04.409247+00	2026-08-12 13:44:04.409247+00	\N	\N
840a1558-373a-43da-9bbb-bd17a6d5dbbc	ffced565-d714-42ce-a4f1-995c9511441c	ticket.asignado	Ticket asignado	El ticket PS-000043 fue asignado: ticket de prueba	medium	IN_APP	PENDIENTE	f	\N	f856d580-0907-4f8f-ae86-7007cf8d527b	\N	2026-08-12 13:44:04.409467+00	2026-08-12 13:44:04.409467+00	\N	\N
a94bb001-9893-4fff-98cd-bc4f5fd942c2	ffced565-d714-42ce-a4f1-995c9511441c	ticket.en_proceso	Ticket en proceso	El técnico ha iniciado la atención del ticket PS-000043: ticket de prueba	medium	IN_APP	PENDIENTE	f	\N	f856d580-0907-4f8f-ae86-7007cf8d527b	\N	2026-08-12 14:03:38.919709+00	2026-08-12 14:03:38.919709+00	\N	\N
095c4f48-7091-4e3c-a7c2-317384418f65	21540103-61cc-4141-a3f1-11763957b648	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000043: ticket de prueba	medium	IN_APP	PENDIENTE	f	\N	f856d580-0907-4f8f-ae86-7007cf8d527b	\N	2026-08-12 14:03:39.112041+00	2026-08-12 14:03:39.112041+00	\N	\N
8f8d5c4e-8314-446e-9813-12e2702cf7ec	ffced565-d714-42ce-a4f1-995c9511441c	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000043: ticket de prueba	medium	IN_APP	PENDIENTE	f	\N	f856d580-0907-4f8f-ae86-7007cf8d527b	\N	2026-08-12 14:03:39.115325+00	2026-08-12 14:03:39.115325+00	\N	\N
8b480849-1f76-4f83-947e-d122fd4a77c5	ffced565-d714-42ce-a4f1-995c9511441c	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000043 está listo para tu validación: ticket de prueba	medium	IN_APP	PENDIENTE	f	\N	f856d580-0907-4f8f-ae86-7007cf8d527b	\N	2026-08-12 14:06:14.717603+00	2026-08-12 14:06:14.717603+00	\N	\N
b5343b0e-b5a7-4874-a82f-2576b5c8abde	ffced565-d714-42ce-a4f1-995c9511441c	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000043 está listo para validación: ticket de prueba	medium	IN_APP	PENDIENTE	f	\N	f856d580-0907-4f8f-ae86-7007cf8d527b	\N	2026-08-12 14:06:14.911753+00	2026-08-12 14:06:14.911753+00	\N	\N
1c3614df-59a0-43d2-bd5e-178fa1dbeb14	21540103-61cc-4141-a3f1-11763957b648	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000043 está listo para validación: ticket de prueba	medium	IN_APP	PENDIENTE	f	\N	f856d580-0907-4f8f-ae86-7007cf8d527b	\N	2026-08-12 14:06:14.917143+00	2026-08-12 14:06:14.917143+00	\N	\N
ead4f34c-ce60-4577-849a-695cb073c3d6	21540103-61cc-4141-a3f1-11763957b648	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000044: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-12 14:26:55.808604+00	2026-08-12 14:26:55.808604+00	\N	\N
c424c792-4c29-4a1b-8bca-b1c63e968779	ffced565-d714-42ce-a4f1-995c9511441c	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000044: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-12 14:26:55.729629+00	2026-08-12 14:26:55.729629+00	\N	\N
025ad45d-1dd2-4c23-999a-fdb5e386e67d	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	ticket.asignado	Ticket asignado	Se te ha asignado el ticket PS-000044: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-12 14:27:31.114788+00	2026-08-12 14:27:31.114788+00	\N	\N
c0114407-f14e-493b-a6ae-ba8da8acd502	ffced565-d714-42ce-a4f1-995c9511441c	ticket.asignado	Ticket asignado	El ticket PS-000044 fue asignado: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-12 14:27:31.223659+00	2026-08-12 14:27:31.223659+00	\N	\N
55fd0eca-837d-4a8a-9570-5ceddb1d63a0	ffced565-d714-42ce-a4f1-995c9511441c	ticket.en_proceso	Ticket en proceso	El técnico ha iniciado la atención del ticket PS-000044: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-17 14:54:38.847718+00	2026-08-17 14:54:38.847718+00	\N	\N
5fc6d951-2285-4625-9bbd-6f4d14593247	ffced565-d714-42ce-a4f1-995c9511441c	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000044: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-17 14:54:39.144604+00	2026-08-17 14:54:39.144604+00	\N	\N
e8ed787f-515f-4acf-9820-b377ef836b12	21540103-61cc-4141-a3f1-11763957b648	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000044: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-17 14:54:39.336422+00	2026-08-17 14:54:39.336422+00	\N	\N
946b4daf-0810-470d-8114-61f685dc4cbf	ffced565-d714-42ce-a4f1-995c9511441c	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000044 está listo para tu validación: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-17 14:55:43.736802+00	2026-08-17 14:55:43.736802+00	\N	\N
cd5de0ea-93dd-433b-85d3-fe9d317318d9	ffced565-d714-42ce-a4f1-995c9511441c	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000044 está listo para validación: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-17 14:55:44.032707+00	2026-08-17 14:55:44.032707+00	\N	\N
abfc379b-ef7c-496f-88ac-4502effd73c3	21540103-61cc-4141-a3f1-11763957b648	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000044 está listo para validación: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-17 14:55:44.03705+00	2026-08-17 14:55:44.03705+00	\N	\N
26c07623-1bd0-4bbf-b35e-ed3c1ecf4e95	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	ticket.rechazado	Ticket rechazado	El ticket PS-000044 ha sido rechazado y está pendiente de reasignación.	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-17 14:58:30.745804+00	2026-08-17 14:58:30.745804+00	\N	\N
c01af34f-9798-4d05-a621-fec44e63db5c	ffced565-d714-42ce-a4f1-995c9511441c	ticket.rechazado	Ticket reabierto	El ticket PS-000044 ha sido rechazado y requiere reasignación.	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-17 14:58:31.054554+00	2026-08-17 14:58:31.054554+00	\N	\N
8d6d7cb4-aebf-4316-a126-8a6f20df41bf	21540103-61cc-4141-a3f1-11763957b648	ticket.asignado	Ticket asignado	El ticket PS-000044 fue asignado: ticket de prueba 23	medium	IN_APP	PENDIENTE	t	2026-08-17 16:30:50.082555+00	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-12 14:27:31.223491+00	2026-08-17 16:30:50.189191+00	\N	\N
15f16315-84ae-4811-9043-aba8412afe8a	21540103-61cc-4141-a3f1-11763957b648	ticket.rechazado	Ticket reabierto	El ticket PS-000044 ha sido rechazado y requiere reasignación.	medium	IN_APP	PENDIENTE	t	2026-08-17 16:31:11.992768+00	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-17 14:58:31.133497+00	2026-08-17 16:31:12.072449+00	\N	\N
889d6b9b-444c-4c3f-bc3b-a42c2b7e9a6e	21540103-61cc-4141-a3f1-11763957b648	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000045: tickte de pruebas 18 de agosto	medium	IN_APP	PENDIENTE	f	\N	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	2026-08-18 17:42:56.733342+00	2026-08-18 17:42:56.733342+00	\N	\N
814fe1df-6c14-4535-b96b-b33b4e13e54d	ffced565-d714-42ce-a4f1-995c9511441c	ticket.nuevo	Nuevo ticket creado	Se creó el ticket PS-000045: tickte de pruebas 18 de agosto	medium	IN_APP	PENDIENTE	f	\N	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	2026-08-18 17:42:56.731328+00	2026-08-18 17:42:56.731328+00	\N	\N
89158f2e-0daa-4b0a-8b52-1fe2e93c6388	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	ticket.asignado	Ticket asignado	Se te ha asignado el ticket PS-000045: tickte de pruebas 18 de agosto	medium	IN_APP	PENDIENTE	f	\N	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	2026-08-18 17:51:21.020244+00	2026-08-18 17:51:21.020244+00	\N	\N
6e22c521-5beb-44b7-bec0-b37b334c175c	ffced565-d714-42ce-a4f1-995c9511441c	ticket.asignado	Ticket asignado	El ticket PS-000045 fue asignado: tickte de pruebas 18 de agosto	medium	IN_APP	PENDIENTE	f	\N	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	2026-08-18 17:51:21.222514+00	2026-08-18 17:51:21.222514+00	\N	\N
21404a32-ddc4-4cbd-b27a-223898f3d2fa	21540103-61cc-4141-a3f1-11763957b648	ticket.asignado	Ticket asignado	El ticket PS-000045 fue asignado: tickte de pruebas 18 de agosto	medium	IN_APP	PENDIENTE	f	\N	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	2026-08-18 17:51:21.228587+00	2026-08-18 17:51:21.228587+00	\N	\N
ee8bbdd7-65fe-4e97-8a5f-2b4d59d8d36f	ffced565-d714-42ce-a4f1-995c9511441c	ticket.en_proceso	Ticket en proceso	El técnico ha iniciado la atención del ticket PS-000045: tickte de pruebas 18 de agosto	medium	IN_APP	PENDIENTE	f	\N	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	2026-08-18 19:37:25.002599+00	2026-08-18 19:37:25.002599+00	\N	\N
2d997480-b797-41ee-bcbc-396a947fc9c2	21540103-61cc-4141-a3f1-11763957b648	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000045: tickte de pruebas 18 de agosto	medium	IN_APP	PENDIENTE	f	\N	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	2026-08-18 19:37:25.621379+00	2026-08-18 19:37:25.621379+00	\N	\N
0da11771-614a-4fcc-9b4d-4c415efb8c78	ffced565-d714-42ce-a4f1-995c9511441c	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000045: tickte de pruebas 18 de agosto	medium	IN_APP	PENDIENTE	f	\N	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	2026-08-18 19:37:25.621178+00	2026-08-18 19:37:25.621178+00	\N	\N
ecc28b63-29e4-4a05-83d8-1ea8555a32b6	ffced565-d714-42ce-a4f1-995c9511441c	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000045 está listo para tu validación: tickte de pruebas 18 de agosto	medium	IN_APP	PENDIENTE	f	\N	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	2026-08-18 19:38:44.207101+00	2026-08-18 19:38:44.207101+00	\N	\N
e8d7f5fb-3b12-4bb9-a863-f9cd1f7e6ef7	ffced565-d714-42ce-a4f1-995c9511441c	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000045 está listo para validación: tickte de pruebas 18 de agosto	medium	IN_APP	PENDIENTE	f	\N	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	2026-08-18 19:38:44.40251+00	2026-08-18 19:38:44.40251+00	\N	\N
30298a34-8260-4b7a-be95-28398cec5aac	21540103-61cc-4141-a3f1-11763957b648	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000045 está listo para validación: tickte de pruebas 18 de agosto	medium	IN_APP	PENDIENTE	f	\N	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	2026-08-18 19:38:44.402863+00	2026-08-18 19:38:44.402863+00	\N	\N
dd8cc563-f88b-47a5-b428-38476ca8870f	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	ticket.cerrado	Ticket cerrado	El ticket PS-000045 ha sido cerrado por el solicitante.	medium	IN_APP	PENDIENTE	f	\N	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	2026-08-18 19:45:59.220672+00	2026-08-18 19:45:59.220672+00	\N	\N
e915ff4b-e870-4866-a5f5-5875df2b6dcb	ffced565-d714-42ce-a4f1-995c9511441c	ticket.cerrado	Ticket cerrado	El ticket PS-000045 fue cerrado: tickte de pruebas 18 de agosto	medium	IN_APP	PENDIENTE	f	\N	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	2026-08-18 19:45:59.311992+00	2026-08-18 19:45:59.311992+00	\N	\N
21b11eda-cdce-4d71-9a9f-4671488aa008	21540103-61cc-4141-a3f1-11763957b648	ticket.cerrado	Ticket cerrado	El ticket PS-000045 fue cerrado: tickte de pruebas 18 de agosto	medium	IN_APP	PENDIENTE	f	\N	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	\N	2026-08-18 19:45:59.312181+00	2026-08-18 19:45:59.312181+00	\N	\N
3b2f4be3-c170-412b-8985-3122ce4cddd5	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	ticket.asignado	Ticket asignado	Se te ha asignado el ticket PS-000044: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-18 19:51:17.10495+00	2026-08-18 19:51:17.10495+00	\N	\N
d9577bec-5cf4-4740-93c6-d21593021bb9	21540103-61cc-4141-a3f1-11763957b648	ticket.asignado	Ticket asignado	El ticket PS-000044 fue asignado: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-18 19:51:17.396429+00	2026-08-18 19:51:17.396429+00	\N	\N
029fd0c8-a751-4e20-b6f3-1cde03dafb5b	ffced565-d714-42ce-a4f1-995c9511441c	ticket.asignado	Ticket asignado	El ticket PS-000044 fue asignado: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-18 19:51:17.309029+00	2026-08-18 19:51:17.309029+00	\N	\N
fe106762-8ef7-488a-96dc-21431a4829da	ffced565-d714-42ce-a4f1-995c9511441c	ticket.en_proceso	Ticket en proceso	El técnico ha iniciado la atención del ticket PS-000044: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-18 19:51:26.308567+00	2026-08-18 19:51:26.308567+00	\N	\N
a5420eac-c47b-40d7-9d72-45049faa716c	ffced565-d714-42ce-a4f1-995c9511441c	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000044: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-18 19:51:26.498731+00	2026-08-18 19:51:26.498731+00	\N	\N
b5029a4d-1217-4e43-8ab6-e523057999e6	21540103-61cc-4141-a3f1-11763957b648	ticket.en_proceso	Ticket en proceso	El técnico inició la atención del ticket PS-000044: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-18 19:51:26.498985+00	2026-08-18 19:51:26.498985+00	\N	\N
fec8810a-246a-4444-9101-141d485c8a4b	ffced565-d714-42ce-a4f1-995c9511441c	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000044 está listo para tu validación: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-18 19:54:31.71901+00	2026-08-18 19:54:31.71901+00	\N	\N
839f5013-81ed-4010-acf0-62e7919e5fdc	21540103-61cc-4141-a3f1-11763957b648	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000044 está listo para validación: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-18 19:54:31.810053+00	2026-08-18 19:54:31.810053+00	\N	\N
bcd2d48f-7376-494c-b334-6e46d41a6286	ffced565-d714-42ce-a4f1-995c9511441c	ticket.pendiente_validacion	Ticket pendiente de validación	El ticket PS-000044 está listo para validación: ticket de prueba 23	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-18 19:54:31.813863+00	2026-08-18 19:54:31.813863+00	\N	\N
83b996f2-cd7e-4896-9e24-f0fe7b28c085	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	ticket.rechazado	Ticket rechazado	El ticket PS-000044 ha sido rechazado y está pendiente de reasignación.	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-18 19:55:36.397341+00	2026-08-18 19:55:36.397341+00	\N	\N
9594a51e-cbd2-4124-9f2d-894babee2bae	ffced565-d714-42ce-a4f1-995c9511441c	ticket.rechazado	Ticket reabierto	El ticket PS-000044 ha sido rechazado y requiere reasignación.	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-18 19:55:36.601333+00	2026-08-18 19:55:36.601333+00	\N	\N
2d46da90-239c-4b6a-bfe9-a6146ac582a9	21540103-61cc-4141-a3f1-11763957b648	ticket.rechazado	Ticket reabierto	El ticket PS-000044 ha sido rechazado y requiere reasignación.	medium	IN_APP	PENDIENTE	f	\N	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	\N	2026-08-18 19:55:36.601108+00	2026-08-18 19:55:36.601108+00	\N	\N
\.


ALTER TABLE public.notificaciones ENABLE TRIGGER ALL;

--
-- Data for Name: parametros_sistema; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.parametros_sistema DISABLE TRIGGER ALL;

COPY public.parametros_sistema (id, empresa_id, clave, valor, tipo_dato, descripcion, created_at, updated_at, updated_by) FROM stdin;
a171c7e9-39b9-494b-833c-7b395b7a8cd7	\N	seguridad.session_timeout_min	480	ENTERO	Tiempo de expiración de sesión en minutos. Default: 8 horas (480 min).	2026-07-13 14:32:54.981359+00	2026-07-13 14:32:54.981359+00	\N
c77cd102-a77e-494c-afbe-8bc0916f6bc7	\N	seguridad.max_intentos_login	5	ENTERO	Número de intentos fallidos de login antes de bloqueo temporal de la cuenta.	2026-07-13 14:32:54.981359+00	2026-07-13 14:32:54.981359+00	\N
827b8905-bbc6-432b-8c55-6ed954a56558	\N	ticket.max_evidencias_por_ticket	10	ENTERO	Número máximo de archivos de evidencia que puede tener un ticket.	2026-07-13 14:32:54.981359+00	2026-07-13 14:32:54.981359+00	\N
d73cc720-2b20-408b-94dd-58d3a32c8bd7	\N	ticket.max_tamano_evidencia_mb	25	ENTERO	Tamaño máximo permitido por archivo de evidencia, en megabytes.	2026-07-13 14:32:54.981359+00	2026-07-13 14:32:54.981359+00	\N
4381a629-5556-4340-8769-78f4a2c580cc	\N	ticket.formatos_evidencia_permitidos	["image/jpeg","image/png","image/webp","application/pdf"]	JSON	Array JSON con los tipos MIME permitidos para archivos de evidencia.	2026-07-13 14:32:54.981359+00	2026-07-13 14:32:54.981359+00	\N
736c8216-ccba-4e5b-8ff9-f73680da848f	\N	ticket.dias_auto_cierre	30	ENTERO	Días sin actividad en PENDIENTE_VALIDACION antes del cierre automático del ticket.	2026-07-13 14:32:54.981359+00	2026-07-13 14:32:54.981359+00	\N
2a888578-f1b7-4e66-bec5-759d65abacb4	\N	notificaciones.email_remitente	noreply@pideservicio.com	TEXTO	Dirección de correo electrónico remitente para notificaciones del sistema.	2026-07-13 14:32:54.981359+00	2026-07-13 14:32:54.981359+00	\N
ac684009-85a9-43a3-8a69-17c760c56705	\N	notificaciones.nombre_remitente	Pide Servicio	TEXTO	Nombre del remitente que aparece en las notificaciones por correo.	2026-07-13 14:32:54.981359+00	2026-07-13 14:32:54.981359+00	\N
ddba9729-7865-4ba5-93da-e60a1e07943c	\N	RESUMEN_DIARIO	true	BOOLEANO	\N	2026-08-01 01:17:04.350629+00	2026-08-01 01:17:04.350629+00	\N
39a6f4b6-ccd9-49f7-9482-d379522e4659	\N	RECORDATORIOS_SLA	true	BOOLEANO	\N	2026-08-01 01:17:04.350638+00	2026-08-01 01:17:04.350638+00	\N
08e8b9a3-48be-4713-82fb-a8e85f5c2d71	\N	NOTIF_EMAIL	false	BOOLEANO	\N	2026-08-01 01:17:04.453293+00	2026-08-01 01:17:04.453293+00	\N
927f8a79-0178-4ff3-bd87-44a6062389ef	aaaaaaaa-0000-0000-0000-000000000001	SESSION_TIMEOUT_MIN	60	ENTERO	\N	2026-07-16 20:14:10.086184+00	2026-07-16 20:14:20.779537+00	21540103-61cc-4141-a3f1-11763957b648
b0a27f57-9a29-4bb8-a2ef-8f21018bb75a	aaaaaaaa-0000-0000-0000-000000000001	AUTH_DOS_FACTORES	true	BOOLEANO	\N	2026-07-16 20:14:10.084058+00	2026-07-16 20:14:20.779538+00	21540103-61cc-4141-a3f1-11763957b648
774e902c-00e8-400f-8fd2-7d6c49ecb38d	aaaaaaaa-0000-0000-0000-000000000001	BLOQUEO_INTENTOS	true	BOOLEANO	\N	2026-07-16 20:14:10.084065+00	2026-07-16 20:14:20.97306+00	21540103-61cc-4141-a3f1-11763957b648
da089abd-84f1-4296-9266-bee123ffde80	\N	ALERTAS_CRITICOS	true	BOOLEANO	\N	2026-08-01 01:17:04.548084+00	2026-08-01 01:17:04.548084+00	\N
b2c8c9d2-2a62-45ab-830b-cc26a8c99100	aaaaaaaa-0000-0000-0000-000000000001	ASIGNACION_AUTOMATICA	false	BOOLEANO	\N	2026-07-16 20:14:28.275381+00	2026-07-16 21:32:40.473339+00	21540103-61cc-4141-a3f1-11763957b648
379b14a7-b859-4206-84c6-f5da0054c3f2	aaaaaaaa-0000-0000-0000-000000000001	PERMITIR_REAPERTURA	true	BOOLEANO	\N	2026-07-16 20:14:27.992265+00	2026-07-16 21:32:40.473148+00	21540103-61cc-4141-a3f1-11763957b648
921360f2-0900-40ee-ad54-12da82204a48	aaaaaaaa-0000-0000-0000-000000000001	MAX_ADJUNTO_MB	10	ENTERO	\N	2026-07-16 20:14:28.182909+00	2026-07-16 21:32:40.474679+00	21540103-61cc-4141-a3f1-11763957b648
04af34f7-7d50-4bb7-98a0-00541136a415	1a3ef008-e768-43f3-9bbd-0be675710cf6	ZONA_HORARIA	America/Lima	TEXTO	\N	2026-07-21 03:23:29.649527+00	2026-07-21 03:23:29.649527+00	\N
5b9c4257-fcf9-4238-adad-aad622a78c96	1a3ef008-e768-43f3-9bbd-0be675710cf6	SESSION_TIMEOUT_MIN	60	ENTERO	\N	2026-07-21 03:20:47.85677+00	2026-07-21 03:23:46.665774+00	21540103-61cc-4141-a3f1-11763957b648
071993c2-6c6f-490f-a895-6594762ad001	1a3ef008-e768-43f3-9bbd-0be675710cf6	AUTH_DOS_FACTORES	false	BOOLEANO	\N	2026-07-21 03:20:47.77536+00	2026-07-21 03:23:46.669134+00	21540103-61cc-4141-a3f1-11763957b648
d94cc139-02ee-4782-933c-ba895dfb1d36	1a3ef008-e768-43f3-9bbd-0be675710cf6	BLOQUEO_INTENTOS	false	BOOLEANO	\N	2026-07-21 03:20:47.85572+00	2026-07-21 03:23:46.670972+00	21540103-61cc-4141-a3f1-11763957b648
2ab459ba-8f37-48a4-8d5b-4592556f6639	\N	PERMITIR_REAPERTURA	false	BOOLEANO	\N	2026-07-21 03:37:23.679586+00	2026-08-01 18:01:04.511413+00	21540103-61cc-4141-a3f1-11763957b648
f1a498c7-283e-4933-a523-dcd9165257be	1a3ef008-e768-43f3-9bbd-0be675710cf6	MODO_MANTENIMIENTO	true	BOOLEANO	\N	2026-07-21 03:23:29.372256+00	2026-08-03 21:40:16.968333+00	ffced565-d714-42ce-a4f1-995c9511441c
4fc431a6-e613-463f-ad9a-f2fd8a586677	1a3ef008-e768-43f3-9bbd-0be675710cf6	ROL_DESC_SUPERADMIN	Acceso total. Gestiona empresas, usuarios, configuración y catálogos globales.	TEXTO	\N	2026-07-27 21:19:53.759215+00	2026-07-27 21:19:53.759215+00	\N
ba983129-c853-4a37-9a99-f8e923b35567	1a3ef008-e768-43f3-9bbd-0be675710cf6	ROL_LABEL_SUPERADMIN	Super Administrador	TEXTO	\N	2026-07-27 21:19:53.665804+00	2026-07-27 21:19:53.665804+00	\N
665c920c-4f74-496e-8edb-70ab856a9d63	\N	ZONA_HORARIA	America/Lima	TEXTO	\N	2026-07-21 03:37:22.783364+00	2026-08-01 01:15:50.344273+00	21540103-61cc-4141-a3f1-11763957b648
71d6ae87-8db9-4b78-9fb0-3834600d057e	\N	MODO_MANTENIMIENTO	false	BOOLEANO	\N	2026-07-21 03:37:22.680497+00	2026-08-06 19:48:34.419975+00	21540103-61cc-4141-a3f1-11763957b648
a23f5398-b34d-4bb3-8353-cc9282ffa1f3	\N	SESSION_TIMEOUT_MIN	120	ENTERO	\N	2026-07-21 03:37:08.176763+00	2026-08-01 01:16:13.045583+00	21540103-61cc-4141-a3f1-11763957b648
d3414cfa-da15-4503-83de-317c791b78b7	\N	AUTH_DOS_FACTORES	true	BOOLEANO	\N	2026-07-21 03:37:08.084096+00	2026-08-01 01:16:13.048092+00	21540103-61cc-4141-a3f1-11763957b648
e1bcbd31-ddcc-42fc-8238-c23cbf4d1a68	\N	BLOQUEO_INTENTOS	true	BOOLEANO	\N	2026-07-21 03:37:08.084071+00	2026-08-01 01:16:13.142564+00	21540103-61cc-4141-a3f1-11763957b648
a5029fda-8b89-4df0-a82d-4f82fda40f13	\N	ASIGNACION_AUTOMATICA	true	BOOLEANO	\N	2026-07-21 03:37:23.876425+00	2026-08-01 01:16:24.342478+00	21540103-61cc-4141-a3f1-11763957b648
cef3c96b-2740-4bfb-94b3-a1c2d0866dde	\N	MAX_ADJUNTO_MB	5	ENTERO	\N	2026-07-21 03:37:23.779469+00	2026-08-01 01:16:24.443989+00	21540103-61cc-4141-a3f1-11763957b648
\.


ALTER TABLE public.parametros_sistema ENABLE TRIGGER ALL;

--
-- Data for Name: permisos; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.permisos DISABLE TRIGGER ALL;

COPY public.permisos (id, codigo, nombre, descripcion, modulo, recurso, accion, activo, created_at, updated_at, created_by, updated_by) FROM stdin;
8391c42e-12f3-46a8-8cbd-b296377fccb9	usuarios.usuario.ver	Ver usuarios	Listar y consultar datos de usuarios de la empresa o del sistema.	usuarios	usuario	ver	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
ebebf206-ea6d-49ed-9bca-0a68b25cc023	usuarios.usuario.crear	Crear usuarios	Crear nuevas cuentas de usuario. Genera la cuenta Auth y el perfil del sistema de forma atómica.	usuarios	usuario	crear	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
e06073a6-2fc8-4a0a-b847-8982e59516d2	usuarios.usuario.editar	Editar usuarios	Modificar datos de perfil, sucursal o área de un usuario existente.	usuarios	usuario	editar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
b4f36303-8a3f-4901-9e06-5203f4b47ac6	usuarios.usuario.desactivar	Desactivar usuarios	Desactivar la cuenta de un usuario. No elimina el registro; bloquea el acceso.	usuarios	usuario	desactivar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
45f98a89-5d1e-434c-b654-8400c6577531	usuarios.usuario.cambiar_rol	Cambiar rol de usuario	Modificar el rol de autorización de un usuario. Requiere rol superior al objetivo.	usuarios	usuario	cambiar_rol	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
29c43b91-7cec-41de-bc95-d2465c036c2f	empresas.empresa.ver	Ver empresas	Listar y consultar datos de empresas registradas en el sistema.	empresas	empresa	ver	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
97477ec1-72d9-4c32-8454-4fafc153cfa8	empresas.empresa.crear	Crear empresas	Registrar una nueva empresa cliente en el sistema.	empresas	empresa	crear	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
2bc1f6a3-f800-4d34-af25-2e1d2bbd8387	empresas.empresa.editar	Editar empresas	Modificar datos de identificación, configuración visual y zona horaria de una empresa.	empresas	empresa	editar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
0e877bc8-02a9-4aef-97b0-0246d4a71a43	empresas.empresa.desactivar	Desactivar empresas	Desactivar una empresa. No permite la desactivación si existen tickets activos asociados.	empresas	empresa	desactivar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
2988aa6d-c1a2-4810-a3e9-531c691b3bbf	sucursales.sucursal.ver	Ver sucursales	Listar y consultar datos de sucursales de la empresa.	sucursales	sucursal	ver	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
d4f5ed40-9d3e-420f-8c16-51a9a00e735c	sucursales.sucursal.crear	Crear sucursales	Registrar una nueva sucursal en la empresa.	sucursales	sucursal	crear	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
5a4d012d-236c-4f25-8c90-0459c5497fc0	sucursales.sucursal.editar	Editar sucursales	Modificar datos y responsable de una sucursal existente.	sucursales	sucursal	editar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
2db91016-72a0-453b-bfad-750330a10ea6	sucursales.sucursal.desactivar	Desactivar sucursales	Desactivar una sucursal. Requiere que no sea la última activa ni tenga tickets activos.	sucursales	sucursal	desactivar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
6a5731a3-96e9-4554-a8ca-416244f3ce8c	areas.area.ver	Ver áreas	Listar y consultar áreas de las sucursales de la empresa.	areas	area	ver	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
4ceb4a31-e27e-47c3-98d7-9ab51a009d9c	areas.area.crear	Crear áreas	Crear una nueva área dentro de una sucursal.	areas	area	crear	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
417cdc41-5943-4196-8023-b98cc62ec54f	areas.area.editar	Editar áreas	Modificar nombre, descripción y responsable de un área.	areas	area	editar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
d3a62626-0bf3-4a73-9c1b-c371ab648506	areas.area.desactivar	Desactivar áreas	Desactivar un área. No permite desactivar si existen tickets activos en ella.	areas	area	desactivar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
0b0a59ac-c417-4492-accc-0cd94890ea15	tipos_servicio.tipo.ver	Ver tipos de servicio	Listar y consultar los tipos de servicio configurados en la empresa.	tipos_servicio	tipo	ver	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
42db400c-b1b9-418a-b981-3f598c2ee251	tipos_servicio.tipo.crear	Crear tipos de servicio	Crear un nuevo tipo de servicio para la empresa.	tipos_servicio	tipo	crear	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
514f82ee-2153-42ba-a0bd-2f1e2c8a25a2	tipos_servicio.tipo.editar	Editar tipos de servicio	Modificar nombre, descripción y orden de un tipo de servicio.	tipos_servicio	tipo	editar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
5abc681e-b8e1-43f7-ae63-8329a426317b	tipos_servicio.tipo.desactivar	Desactivar tipos de servicio	Desactivar un tipo de servicio. Los tickets existentes no se ven afectados.	tipos_servicio	tipo	desactivar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
3d8e1d85-ca77-4629-ba5f-7d21b9c83cd6	categorias.categoria.ver	Ver categorías	Listar y consultar categorías globales y de la empresa.	categorias	categoria	ver	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
8e1d1e2e-390c-4f21-9fc7-597fda520d01	categorias.categoria.crear	Crear categorías	Crear una nueva categoría de ticket.	categorias	categoria	crear	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
5fd8bf12-dce4-4c43-add4-66d06cbd43c0	categorias.categoria.editar	Editar categorías	Modificar nombre y descripción de una categoría.	categorias	categoria	editar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
b1543e70-f734-4fb1-bd43-acd9a85d2c66	categorias.categoria.desactivar	Desactivar categorías	Desactivar una categoría. Los tickets existentes no se ven afectados.	categorias	categoria	desactivar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
6010099c-1920-480a-b700-4410e24b4cff	sla.configuracion.ver	Ver configuraciones SLA	Consultar la matriz SLA configurada para la empresa.	sla	configuracion	ver	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
a8b41d28-2fe9-4eb0-b430-293f3132c6ff	sla.configuracion.crear	Crear configuraciones SLA	Definir una nueva regla SLA para la combinación empresa × tipo_servicio × prioridad.	sla	configuracion	crear	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
cf71dd83-ad72-43f7-95e7-c723287e63b8	sla.configuracion.editar	Editar configuraciones SLA	Modificar tiempos, horario y días laborables de una regla SLA existente.	sla	configuracion	editar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
5ef76e7c-d902-4f03-b938-559362b668b4	sla.configuracion.desactivar	Desactivar configuraciones SLA	Desactivar una regla SLA. No afecta fechas límite ya calculadas en tickets.	sla	configuracion	desactivar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
0257ab8b-4e70-453f-9a6e-fe784415c562	feriados.feriado.ver	Ver feriados	Consultar el calendario de feriados configurado para la empresa.	feriados	feriado	ver	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
2003b1ce-1f70-446d-9915-5efa04f90dcf	feriados.feriado.crear	Crear feriados	Registrar un nuevo feriado en el calendario de la empresa.	feriados	feriado	crear	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
06fc223e-dc3c-466c-b3cc-fd5aab7b2b08	feriados.feriado.editar	Editar feriados	Modificar nombre y fecha de un feriado registrado.	feriados	feriado	editar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
8c122d4e-371c-47c1-914a-e7f2137765a0	feriados.feriado.eliminar	Eliminar feriados	Eliminar un feriado del calendario. Operación de borrado lógico.	feriados	feriado	eliminar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
d4e1a5da-39a2-4254-b9d1-c21dbfc784c9	motivos_cancelacion.motivo.ver	Ver motivos de cancelación	Consultar el catálogo de motivos de cancelación disponibles.	motivos_cancelacion	motivo	ver	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
2600d9da-0099-461a-a6c3-e86a27e5cc34	motivos_cancelacion.motivo.gestionar	Gestionar motivos de cancelación	Crear, editar y desactivar motivos de cancelación de la empresa.	motivos_cancelacion	motivo	gestionar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
0d914107-09dc-4fe8-a8be-65f30bcbfeb8	motivos_rechazo.motivo.ver	Ver motivos de rechazo	Consultar el catálogo de motivos de rechazo disponibles.	motivos_rechazo	motivo	ver	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
93c07834-8c14-4959-b569-7753feeb41aa	motivos_rechazo.motivo.gestionar	Gestionar motivos de rechazo	Crear, editar y desactivar motivos de rechazo de la empresa.	motivos_rechazo	motivo	gestionar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
8dbf4216-c1f4-4dc7-b359-ba95da111c81	tickets.ticket.crear	Crear tickets	Registrar un nuevo ticket de solicitud, incidencia o riesgo.	tickets	ticket	crear	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
b0e5ded5-10f6-47ab-99f2-0661df9c3fe6	tickets.ticket.ver	Ver tickets propios	Consultar tickets propios o asignados al usuario.	tickets	ticket	ver	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
6a40a339-b2f0-487e-bb57-417c9c968942	tickets.ticket.ver_todos	Ver todos los tickets	Consultar todos los tickets de la empresa o sucursal sin restricción de propiedad.	tickets	ticket	ver_todos	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
360e978a-0e65-4cee-ba98-6db45da12c02	tickets.ticket.asignar	Asignar tickets	Asignar un técnico a un ticket en estado SIN_ASIGNAR.	tickets	ticket	asignar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
2e5ab04a-4739-47d6-b849-553a46d070b9	tickets.ticket.reasignar	Reasignar tickets	Cambiar el técnico asignado a un ticket activo con auditoría.	tickets	ticket	reasignar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
17bc166f-cd4f-4a7d-b3c8-57ca6cd26aeb	tickets.ticket.iniciar	Iniciar proceso de ticket	Transicionar el ticket de ASIGNADO a EN_PROCESO.	tickets	ticket	iniciar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
85d35dd8-4c5b-47a4-a20a-b97622254ca1	tickets.ticket.pausar	Pausar ticket	Transicionar el ticket de EN_PROCESO a EN_ESPERA con justificación.	tickets	ticket	pausar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
e47f0e7a-f10e-43f9-ac63-ec2e6f5733a2	tickets.ticket.reanudar	Reanudar ticket	Transicionar el ticket de EN_ESPERA a EN_PROCESO.	tickets	ticket	reanudar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
3086e507-2971-45f4-a704-e309719b09ba	tickets.ticket.completar	Completar ticket	Transicionar el ticket de EN_PROCESO a PENDIENTE_VALIDACION.	tickets	ticket	completar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
12a401d2-558e-4d4b-92c3-b201ccc33a15	tickets.ticket.validar	Validar y cerrar ticket	Aprobar la resolución del ticket. Transiciona a CERRADO.	tickets	ticket	validar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
8bd7c3d8-230a-4810-8000-d43c995d9114	tickets.ticket.reabrir	Reabrir ticket rechazado	Rechazar la resolución con motivo. Transiciona a REABIERTO.	tickets	ticket	reabrir	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
91495fe3-73fc-414e-993b-5efe6e62b395	tickets.ticket.cancelar	Cancelar ticket	Cancelar un ticket activo con motivo obligatorio. Estado final.	tickets	ticket	cancelar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
9bc711f1-7740-4206-bda2-049f7ca1ce05	tickets.ticket.cambiar_prioridad	Cambiar prioridad del ticket	Modificar prioridad_admin del ticket. Actualiza prioridad_efectiva con auditoría.	tickets	ticket	cambiar_prioridad	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
afd619c5-8264-48d4-8004-4b0741005322	tickets.ticket.cambiar_area	Cambiar área del ticket	Modificar el área de un ticket. Libre en SIN_ASIGNAR; requiere privilegios y auditoría en otros estados.	tickets	ticket	cambiar_area	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
1c2b0209-87f2-40e7-9273-e3019bcf68f8	tickets.ticket.valorar	Valorar resolución del ticket	Registrar la valoración de satisfacción (1 a 5) del solicitante al cerrar el ticket.	tickets	ticket	valorar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
aebcf5e2-eec2-402f-bc83-57d463da7891	tickets.comentario.crear	Crear comentarios en ticket	Agregar un comentario al hilo del ticket.	tickets	comentario	crear	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
b44a8a35-df1e-40ed-978c-b244bed932c5	tickets.evidencia.subir	Subir evidencias al ticket	Adjuntar archivos de evidencia (imágenes, PDF) al ticket.	tickets	evidencia	subir	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
49e75bbb-0f2f-4a4b-9af5-a10af4471e31	notificaciones.notificacion.ver	Ver notificaciones	Consultar las notificaciones propias del usuario.	notificaciones	notificacion	ver	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
baf374ab-ef8d-44bb-b10c-1a5ac600d15e	notificaciones.notificacion.marcar_leida	Marcar notificaciones como leídas	Marcar una o todas las notificaciones propias como leídas.	notificaciones	notificacion	marcar_leida	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
fbff25ca-7583-4bc8-92e8-e171296e808c	notificaciones.preferencia.editar	Editar preferencias de notificación	Configurar qué tipos de eventos generan notificación y por qué canal.	notificaciones	preferencia	editar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
55aa2940-8396-4410-86ea-073773f3713c	reportes.reporte.ver	Ver reportes	Acceder al módulo de reportes y visualizar métricas y gráficas.	reportes	reporte	ver	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
23ebd915-4f1e-4293-9e7e-036a88ff6167	reportes.reporte.exportar	Exportar reportes	Descargar reportes en formato CSV, Excel o PDF.	reportes	reporte	exportar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
01a16ecc-9b01-4d55-a84d-e9adb1e3a05d	auditoria.log.ver	Ver logs de auditoría	Consultar el registro de auditoría del sistema (audit_logs).	auditoria	log	ver	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
255418fb-b240-4053-82bf-369e6e972333	configuracion.parametro.ver	Ver parámetros del sistema	Consultar la configuración clave-valor global y por empresa.	configuracion	parametro	ver	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
ea8ea4f2-f0b8-4576-b1b1-578e4b3b1994	configuracion.parametro.editar	Editar parámetros del sistema	Modificar los valores de configuración del sistema.	configuracion	parametro	editar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
8b74d13b-d61d-4fc4-8e9e-4b8a291bf9bc	configuracion.rbac.gestionar	Gestionar permisos y roles	Asignar y revocar permisos a roles (role_permissions). Solo SuperAdmin.	configuracion	rbac	gestionar	t	2026-07-13 14:32:54.319852+00	2026-07-13 14:32:54.319852+00	\N	\N
\.


ALTER TABLE public.permisos ENABLE TRIGGER ALL;

--
-- Data for Name: preferencias_notificacion; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.preferencias_notificacion DISABLE TRIGGER ALL;

COPY public.preferencias_notificacion (id, usuario_id, canal_in_app, canal_email, canal_push, evento_ticket_asignado, evento_ticket_actualizado, evento_comentario_nuevo, evento_sla_vencido, evento_ticket_reabierto, resumen_diario, modo_silencioso, silencio_hora_inicio, silencio_hora_fin, created_at, updated_at, updated_by) FROM stdin;
86fd78e6-6352-4b60-bf67-caf43935b6e7	02419c75-3006-4f51-8019-a435201f52ba	t	t	f	t	t	t	t	t	f	f	\N	\N	2026-07-17 20:41:30.015884+00	2026-07-17 20:41:30.015884+00	\N
94ed893c-9d4a-459a-abe2-cc699026cf1c	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	t	t	f	t	t	t	t	t	f	f	\N	\N	2026-07-17 20:41:56.335936+00	2026-07-17 20:41:56.335936+00	\N
f3005ca9-e4ad-4f24-b440-8ddb039ad902	ffced565-d714-42ce-a4f1-995c9511441c	t	t	f	t	t	t	t	t	f	f	\N	\N	2026-07-21 14:15:22.076035+00	2026-07-21 14:15:22.076035+00	\N
eaf40707-a20a-459d-963c-3b9de29c7218	25541c55-aafe-4714-98d1-a177b057302e	t	t	f	t	t	t	t	t	f	f	\N	\N	2026-08-03 15:57:40.99677+00	2026-08-03 15:57:40.99677+00	\N
40cd5d97-ba45-4b3b-a9e7-f60ed8e5ea56	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	t	t	f	t	t	t	t	t	f	f	\N	\N	2026-08-04 17:01:48.020622+00	2026-08-04 17:01:48.020622+00	\N
\.


ALTER TABLE public.preferencias_notificacion ENABLE TRIGGER ALL;

--
-- Data for Name: roles; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.roles DISABLE TRIGGER ALL;

COPY public.roles (id, codigo, nombre, descripcion, activo, created_at, updated_at, created_by, updated_by) FROM stdin;
f5d9d58a-de7b-47a9-abee-d8d551374cdf	SUPERADMIN	SuperAdministrador	Acceso total al sistema. Gestiona empresas, usuarios globales, catálogos globales y parámetros del sistema.	t	2026-07-13 14:32:54.142937+00	2026-07-13 14:32:54.142937+00	\N	\N
98ec4c10-c0b0-45cc-bcac-17187011edc8	ADMIN	Administrador	Gestiona su empresa: usuarios, sucursales, áreas, tipos de servicio, SLA, catálogos, reportes completos y configuración.	t	2026-07-13 14:32:54.142937+00	2026-07-13 14:32:54.142937+00	\N	\N
54abee54-0a82-4c1d-893c-d5655fecd941	SUPERVISOR	Supervisor	Supervisa la operación de su sucursal. En MVP v1.0 tiene los mismos accesos operativos que Administrador a nivel de sucursal.	t	2026-07-13 14:32:54.142937+00	2026-07-13 14:32:54.142937+00	\N	\N
58fbc153-1c08-4b63-a35a-2cf27ff938ae	TECNICO	Técnico	Ejecuta y resuelve los tickets asignados. Puede pertenecer a múltiples sucursales. Estado laboral ACTIVO requerido para recibir asignaciones.	t	2026-07-13 14:32:54.142937+00	2026-07-13 14:32:54.142937+00	\N	\N
2ba41625-7c48-4331-ba5e-72bad3825fd7	TRABAJADOR	Trabajador	Crea tickets y realiza seguimiento de los suyos. Puede validar la resolución o rechazarla con motivo justificado.	t	2026-07-13 14:32:54.142937+00	2026-07-13 14:32:54.142937+00	\N	\N
9ffa57e6-ef77-457e-b23d-24b66762dfb1	USUARIO	Usuario	Rol básico. Solo puede crear tickets propios y consultar su estado.	t	2026-07-13 14:32:54.142937+00	2026-07-13 14:32:54.142937+00	\N	\N
\.


ALTER TABLE public.roles ENABLE TRIGGER ALL;

--
-- Data for Name: role_permissions; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.role_permissions DISABLE TRIGGER ALL;

COPY public.role_permissions (id, rol_codigo, permiso_id, empresa_id, activo, created_at, created_by) FROM stdin;
c526d35f-3022-431c-8f77-78d58e98df61	SUPERADMIN	8391c42e-12f3-46a8-8cbd-b296377fccb9	\N	t	2026-07-13 14:32:58.390548+00	\N
84dff712-ec00-4410-80b5-fadb69bbbcef	SUPERADMIN	ebebf206-ea6d-49ed-9bca-0a68b25cc023	\N	t	2026-07-13 14:32:58.390548+00	\N
7dcc200a-3abb-4e32-bb96-abb8fb2a2584	SUPERADMIN	e06073a6-2fc8-4a0a-b847-8982e59516d2	\N	t	2026-07-13 14:32:58.390548+00	\N
8364ca08-d46d-4e09-b7d5-482f78feb32d	SUPERADMIN	b4f36303-8a3f-4901-9e06-5203f4b47ac6	\N	t	2026-07-13 14:32:58.390548+00	\N
5f84c8b4-fe49-4216-879f-ef1489bf4953	SUPERADMIN	45f98a89-5d1e-434c-b654-8400c6577531	\N	t	2026-07-13 14:32:58.390548+00	\N
953d9bd7-dbec-4fda-985a-16feb5d25b2e	SUPERADMIN	29c43b91-7cec-41de-bc95-d2465c036c2f	\N	t	2026-07-13 14:32:58.390548+00	\N
dbdf2186-da8a-48d6-bd07-469e9a51b4a3	SUPERADMIN	97477ec1-72d9-4c32-8454-4fafc153cfa8	\N	t	2026-07-13 14:32:58.390548+00	\N
e68a1b5d-b424-4c5e-9aaa-f2232d7c5bd0	SUPERADMIN	2bc1f6a3-f800-4d34-af25-2e1d2bbd8387	\N	t	2026-07-13 14:32:58.390548+00	\N
6cf2c44e-5350-438c-a9fe-3b211de86207	SUPERADMIN	0e877bc8-02a9-4aef-97b0-0246d4a71a43	\N	t	2026-07-13 14:32:58.390548+00	\N
6cacfe4c-b8c6-43bb-8b71-7100f8348d73	SUPERADMIN	2988aa6d-c1a2-4810-a3e9-531c691b3bbf	\N	t	2026-07-13 14:32:58.390548+00	\N
68d5386f-cb78-4f96-8b45-bc649f0480ee	SUPERADMIN	d4f5ed40-9d3e-420f-8c16-51a9a00e735c	\N	t	2026-07-13 14:32:58.390548+00	\N
3b4815b1-92c7-44f2-9d0a-8811120826fc	SUPERADMIN	5a4d012d-236c-4f25-8c90-0459c5497fc0	\N	t	2026-07-13 14:32:58.390548+00	\N
3e197837-b3f4-42fe-88a3-0918ab168f8f	SUPERADMIN	2db91016-72a0-453b-bfad-750330a10ea6	\N	t	2026-07-13 14:32:58.390548+00	\N
67bdccdb-a336-4227-a6e0-4569cc4c7e47	SUPERADMIN	6a5731a3-96e9-4554-a8ca-416244f3ce8c	\N	t	2026-07-13 14:32:58.390548+00	\N
45d71638-7f4c-44de-8123-fa2dd568d516	SUPERADMIN	4ceb4a31-e27e-47c3-98d7-9ab51a009d9c	\N	t	2026-07-13 14:32:58.390548+00	\N
ad720b1f-123e-493b-b197-e4421cf6b5b5	SUPERADMIN	417cdc41-5943-4196-8023-b98cc62ec54f	\N	t	2026-07-13 14:32:58.390548+00	\N
e3d6643c-cd75-4a5c-8434-18391fa5bde6	SUPERADMIN	d3a62626-0bf3-4a73-9c1b-c371ab648506	\N	t	2026-07-13 14:32:58.390548+00	\N
5f7903b9-b1e3-438f-a480-8e48b4c1a2ba	SUPERADMIN	0b0a59ac-c417-4492-accc-0cd94890ea15	\N	t	2026-07-13 14:32:58.390548+00	\N
6b934cce-e64f-4313-851a-456c04d0d70b	SUPERADMIN	42db400c-b1b9-418a-b981-3f598c2ee251	\N	t	2026-07-13 14:32:58.390548+00	\N
5bd07b68-2e3c-4c48-a3fe-48a4e0f3c6b1	SUPERADMIN	514f82ee-2153-42ba-a0bd-2f1e2c8a25a2	\N	t	2026-07-13 14:32:58.390548+00	\N
8e882599-2624-49fe-b519-27cc232641bf	SUPERADMIN	5abc681e-b8e1-43f7-ae63-8329a426317b	\N	t	2026-07-13 14:32:58.390548+00	\N
94648277-edb6-4689-b92d-cdf5d5844e33	SUPERADMIN	3d8e1d85-ca77-4629-ba5f-7d21b9c83cd6	\N	t	2026-07-13 14:32:58.390548+00	\N
3b2fc900-7bf6-41c7-9cc0-aaf589c7abaa	SUPERADMIN	8e1d1e2e-390c-4f21-9fc7-597fda520d01	\N	t	2026-07-13 14:32:58.390548+00	\N
54aefecd-f7c0-475b-a440-009e49b530ab	SUPERADMIN	5fd8bf12-dce4-4c43-add4-66d06cbd43c0	\N	t	2026-07-13 14:32:58.390548+00	\N
e3fda9e8-bb0a-49ab-8086-735760e1677d	SUPERADMIN	b1543e70-f734-4fb1-bd43-acd9a85d2c66	\N	t	2026-07-13 14:32:58.390548+00	\N
9e57b907-6073-47dd-b8e9-c797642e4c29	SUPERADMIN	6010099c-1920-480a-b700-4410e24b4cff	\N	t	2026-07-13 14:32:58.390548+00	\N
43b98266-187a-4ec7-bf8c-19100664c4fa	SUPERADMIN	a8b41d28-2fe9-4eb0-b430-293f3132c6ff	\N	t	2026-07-13 14:32:58.390548+00	\N
7a55bb8f-7333-4f2f-9502-bb904addbbd0	SUPERADMIN	cf71dd83-ad72-43f7-95e7-c723287e63b8	\N	t	2026-07-13 14:32:58.390548+00	\N
37614df5-2a28-4a73-bc40-551542c87f77	SUPERADMIN	5ef76e7c-d902-4f03-b938-559362b668b4	\N	t	2026-07-13 14:32:58.390548+00	\N
41b248a3-052a-4643-84ba-526c6571de1b	SUPERADMIN	0257ab8b-4e70-453f-9a6e-fe784415c562	\N	t	2026-07-13 14:32:58.390548+00	\N
e4219f08-6598-45a6-9230-f281a2d781b3	SUPERADMIN	2003b1ce-1f70-446d-9915-5efa04f90dcf	\N	t	2026-07-13 14:32:58.390548+00	\N
2a5725a1-e0f1-45c9-a432-df8693116940	SUPERADMIN	06fc223e-dc3c-466c-b3cc-fd5aab7b2b08	\N	t	2026-07-13 14:32:58.390548+00	\N
cdf352f5-e6fd-4d17-bace-b3776426e34c	SUPERADMIN	8c122d4e-371c-47c1-914a-e7f2137765a0	\N	t	2026-07-13 14:32:58.390548+00	\N
50640a3c-dab7-4cbc-9dea-9467abaea884	SUPERADMIN	d4e1a5da-39a2-4254-b9d1-c21dbfc784c9	\N	t	2026-07-13 14:32:58.390548+00	\N
718a5199-bcf3-47a0-bcc0-9a5c8def729f	SUPERADMIN	2600d9da-0099-461a-a6c3-e86a27e5cc34	\N	t	2026-07-13 14:32:58.390548+00	\N
597baec9-7da8-42d0-92ce-bfcc6891b3aa	SUPERADMIN	0d914107-09dc-4fe8-a8be-65f30bcbfeb8	\N	t	2026-07-13 14:32:58.390548+00	\N
f373db7d-1270-435b-8956-22925e25103f	SUPERADMIN	93c07834-8c14-4959-b569-7753feeb41aa	\N	t	2026-07-13 14:32:58.390548+00	\N
603ba4fa-d60c-4200-9a28-7dbeb68f89a3	SUPERADMIN	8dbf4216-c1f4-4dc7-b359-ba95da111c81	\N	t	2026-07-13 14:32:58.390548+00	\N
6dd0f03f-c3bd-42b8-a09e-de3d7fad6a5b	SUPERADMIN	b0e5ded5-10f6-47ab-99f2-0661df9c3fe6	\N	t	2026-07-13 14:32:58.390548+00	\N
43839449-52f6-4b61-b3ac-e80bb2a30628	SUPERADMIN	6a40a339-b2f0-487e-bb57-417c9c968942	\N	t	2026-07-13 14:32:58.390548+00	\N
3743f199-14d7-47bc-b7b2-0003f9b8ddd8	SUPERADMIN	360e978a-0e65-4cee-ba98-6db45da12c02	\N	t	2026-07-13 14:32:58.390548+00	\N
5c7abc95-944c-4d0a-8190-31dbbd113f5c	SUPERADMIN	2e5ab04a-4739-47d6-b849-553a46d070b9	\N	t	2026-07-13 14:32:58.390548+00	\N
0ab0e988-eafe-4610-be45-af62ee7323dd	SUPERADMIN	17bc166f-cd4f-4a7d-b3c8-57ca6cd26aeb	\N	t	2026-07-13 14:32:58.390548+00	\N
d8f95b24-9d11-45e7-b6dc-455763daf2a0	SUPERADMIN	85d35dd8-4c5b-47a4-a20a-b97622254ca1	\N	t	2026-07-13 14:32:58.390548+00	\N
661a745a-8f1e-4379-96a2-b8173f8489d5	SUPERADMIN	e47f0e7a-f10e-43f9-ac63-ec2e6f5733a2	\N	t	2026-07-13 14:32:58.390548+00	\N
dff48b41-41d3-4e00-a396-c6e1925ab502	SUPERADMIN	3086e507-2971-45f4-a704-e309719b09ba	\N	t	2026-07-13 14:32:58.390548+00	\N
9c3aed99-c296-4a37-864e-14f941aa76f7	SUPERADMIN	12a401d2-558e-4d4b-92c3-b201ccc33a15	\N	t	2026-07-13 14:32:58.390548+00	\N
8519cd22-79eb-45bb-9387-b17495a11c00	SUPERADMIN	8bd7c3d8-230a-4810-8000-d43c995d9114	\N	t	2026-07-13 14:32:58.390548+00	\N
61f4b5e5-8f62-4827-bb40-9d60e85aff7a	SUPERADMIN	91495fe3-73fc-414e-993b-5efe6e62b395	\N	t	2026-07-13 14:32:58.390548+00	\N
f343b3fb-e325-4ed3-94b8-9fae15bc0930	SUPERADMIN	9bc711f1-7740-4206-bda2-049f7ca1ce05	\N	t	2026-07-13 14:32:58.390548+00	\N
2017879c-2c26-4082-bdab-ba6c6b7c0632	SUPERADMIN	afd619c5-8264-48d4-8004-4b0741005322	\N	t	2026-07-13 14:32:58.390548+00	\N
b4d9bd2b-66c0-47b4-a82a-accb00a8166c	SUPERADMIN	1c2b0209-87f2-40e7-9273-e3019bcf68f8	\N	t	2026-07-13 14:32:58.390548+00	\N
82e1ec9b-18f2-4619-aa0c-664b128ba637	SUPERADMIN	aebcf5e2-eec2-402f-bc83-57d463da7891	\N	t	2026-07-13 14:32:58.390548+00	\N
2dee932d-ea59-42f4-90f1-7751b2462a5d	SUPERADMIN	b44a8a35-df1e-40ed-978c-b244bed932c5	\N	t	2026-07-13 14:32:58.390548+00	\N
d5c43d42-05cb-441f-bce7-358a0aeb36c9	SUPERADMIN	49e75bbb-0f2f-4a4b-9af5-a10af4471e31	\N	t	2026-07-13 14:32:58.390548+00	\N
854d84be-7256-4fd5-b668-2cfc4aea7a61	SUPERADMIN	baf374ab-ef8d-44bb-b10c-1a5ac600d15e	\N	t	2026-07-13 14:32:58.390548+00	\N
f5fdb436-2f30-4459-8431-1453fb8bbcf2	SUPERADMIN	fbff25ca-7583-4bc8-92e8-e171296e808c	\N	t	2026-07-13 14:32:58.390548+00	\N
e75c7f51-fb54-406d-a7d2-005bdfed5663	SUPERADMIN	55aa2940-8396-4410-86ea-073773f3713c	\N	t	2026-07-13 14:32:58.390548+00	\N
b432b138-ff27-4244-97e5-73188ba1a21f	SUPERADMIN	23ebd915-4f1e-4293-9e7e-036a88ff6167	\N	t	2026-07-13 14:32:58.390548+00	\N
738136b8-14c7-48dd-847e-20536c04d190	SUPERADMIN	01a16ecc-9b01-4d55-a84d-e9adb1e3a05d	\N	t	2026-07-13 14:32:58.390548+00	\N
a7565c35-34e7-43da-a409-990db481986b	SUPERADMIN	255418fb-b240-4053-82bf-369e6e972333	\N	t	2026-07-13 14:32:58.390548+00	\N
bf634def-1416-4512-bc6b-854c661514f0	SUPERADMIN	ea8ea4f2-f0b8-4576-b1b1-578e4b3b1994	\N	t	2026-07-13 14:32:58.390548+00	\N
d0571e23-1caa-4774-b552-b3d1e458a11e	SUPERADMIN	8b74d13b-d61d-4fc4-8e9e-4b8a291bf9bc	\N	t	2026-07-13 14:32:58.390548+00	\N
1a9e87bd-42d2-49fe-905e-d63d650804f5	ADMIN	8391c42e-12f3-46a8-8cbd-b296377fccb9	\N	t	2026-07-13 14:32:58.559726+00	\N
63d64247-6eb3-43ed-a06e-2830c337aabe	ADMIN	ebebf206-ea6d-49ed-9bca-0a68b25cc023	\N	t	2026-07-13 14:32:58.559726+00	\N
d485b646-c579-4c6d-a9b5-beba00105977	ADMIN	e06073a6-2fc8-4a0a-b847-8982e59516d2	\N	t	2026-07-13 14:32:58.559726+00	\N
ed65b797-a539-43d1-9761-2f7eaa85caa5	ADMIN	b4f36303-8a3f-4901-9e06-5203f4b47ac6	\N	t	2026-07-13 14:32:58.559726+00	\N
c3e4ec4e-9a6f-407d-b7f4-53b9f539ad3f	ADMIN	45f98a89-5d1e-434c-b654-8400c6577531	\N	t	2026-07-13 14:32:58.559726+00	\N
62c86d22-6fd1-4472-b169-54f1cc1991f6	ADMIN	29c43b91-7cec-41de-bc95-d2465c036c2f	\N	t	2026-07-13 14:32:58.559726+00	\N
9acbab15-c84a-40c5-8f26-f46ce6794445	ADMIN	2bc1f6a3-f800-4d34-af25-2e1d2bbd8387	\N	t	2026-07-13 14:32:58.559726+00	\N
20563469-adfa-4431-99b1-1e93384bd8a3	ADMIN	2988aa6d-c1a2-4810-a3e9-531c691b3bbf	\N	t	2026-07-13 14:32:58.559726+00	\N
b8c57488-8b52-4abc-8ca0-6ebb63249f97	ADMIN	d4f5ed40-9d3e-420f-8c16-51a9a00e735c	\N	t	2026-07-13 14:32:58.559726+00	\N
42f3b60c-a4f0-431c-a811-01dae33761a5	ADMIN	5a4d012d-236c-4f25-8c90-0459c5497fc0	\N	t	2026-07-13 14:32:58.559726+00	\N
a2145968-b742-4db9-84fd-c3f3b3c83baa	ADMIN	2db91016-72a0-453b-bfad-750330a10ea6	\N	t	2026-07-13 14:32:58.559726+00	\N
3a1c2c2a-f58e-4c57-aee8-4c65a16a7033	ADMIN	6a5731a3-96e9-4554-a8ca-416244f3ce8c	\N	t	2026-07-13 14:32:58.559726+00	\N
9b62f8db-7c17-4605-a9ec-c0e4c5eff150	ADMIN	4ceb4a31-e27e-47c3-98d7-9ab51a009d9c	\N	t	2026-07-13 14:32:58.559726+00	\N
9a59b63e-cacb-49f2-872b-ae8c49749885	ADMIN	417cdc41-5943-4196-8023-b98cc62ec54f	\N	t	2026-07-13 14:32:58.559726+00	\N
930cb030-d514-478d-b32f-56c2648c28f2	ADMIN	d3a62626-0bf3-4a73-9c1b-c371ab648506	\N	t	2026-07-13 14:32:58.559726+00	\N
39fbdecf-8caf-4c78-a41a-6bc655cc1d35	ADMIN	0b0a59ac-c417-4492-accc-0cd94890ea15	\N	t	2026-07-13 14:32:58.559726+00	\N
fb053698-5acb-4d7c-9fa2-053396c537e0	ADMIN	42db400c-b1b9-418a-b981-3f598c2ee251	\N	t	2026-07-13 14:32:58.559726+00	\N
347c04df-63ad-4bc2-939f-66a19604b008	ADMIN	514f82ee-2153-42ba-a0bd-2f1e2c8a25a2	\N	t	2026-07-13 14:32:58.559726+00	\N
f5e3aab2-840e-42c3-a512-caa7cdda5cad	ADMIN	5abc681e-b8e1-43f7-ae63-8329a426317b	\N	t	2026-07-13 14:32:58.559726+00	\N
234209cc-e4bd-4ae2-98fa-663f82e7f43f	ADMIN	3d8e1d85-ca77-4629-ba5f-7d21b9c83cd6	\N	t	2026-07-13 14:32:58.559726+00	\N
2dff0daa-f842-4004-b0c2-2c0497ef8699	ADMIN	8e1d1e2e-390c-4f21-9fc7-597fda520d01	\N	t	2026-07-13 14:32:58.559726+00	\N
d6bde3f1-3722-4808-8c05-1662ec7e7aad	ADMIN	5fd8bf12-dce4-4c43-add4-66d06cbd43c0	\N	t	2026-07-13 14:32:58.559726+00	\N
8ba93549-f60e-4c72-b0bc-10da3c6fc842	ADMIN	b1543e70-f734-4fb1-bd43-acd9a85d2c66	\N	t	2026-07-13 14:32:58.559726+00	\N
ed4c850c-b8c2-416a-bd3b-4f6d175f615e	ADMIN	6010099c-1920-480a-b700-4410e24b4cff	\N	t	2026-07-13 14:32:58.559726+00	\N
d0cd1c08-6c37-4e04-b8ee-1eb719ac8803	ADMIN	a8b41d28-2fe9-4eb0-b430-293f3132c6ff	\N	t	2026-07-13 14:32:58.559726+00	\N
3b6660eb-5ce0-462f-a2ac-ae523141caa2	ADMIN	cf71dd83-ad72-43f7-95e7-c723287e63b8	\N	t	2026-07-13 14:32:58.559726+00	\N
7bd18a9d-8845-44bc-9c00-4bd94744133b	ADMIN	5ef76e7c-d902-4f03-b938-559362b668b4	\N	t	2026-07-13 14:32:58.559726+00	\N
28e81b1a-cc68-4d79-8042-3e8e4ee02823	ADMIN	0257ab8b-4e70-453f-9a6e-fe784415c562	\N	t	2026-07-13 14:32:58.559726+00	\N
b43a7f3d-03be-48bd-b172-aaaa463af8b1	ADMIN	2003b1ce-1f70-446d-9915-5efa04f90dcf	\N	t	2026-07-13 14:32:58.559726+00	\N
b65a05b1-6213-4f16-8835-e9cf959c6db5	ADMIN	06fc223e-dc3c-466c-b3cc-fd5aab7b2b08	\N	t	2026-07-13 14:32:58.559726+00	\N
348cae7e-90cc-41ae-8d6b-8aafeae8110b	ADMIN	8c122d4e-371c-47c1-914a-e7f2137765a0	\N	t	2026-07-13 14:32:58.559726+00	\N
3d8409b7-cf42-4a52-98ba-ddc1762fd035	ADMIN	d4e1a5da-39a2-4254-b9d1-c21dbfc784c9	\N	t	2026-07-13 14:32:58.559726+00	\N
fc328e3a-37d0-4680-b7c5-b1ccab22d184	ADMIN	2600d9da-0099-461a-a6c3-e86a27e5cc34	\N	t	2026-07-13 14:32:58.559726+00	\N
e94584b0-be07-4b3a-a88e-0854030c704f	ADMIN	0d914107-09dc-4fe8-a8be-65f30bcbfeb8	\N	t	2026-07-13 14:32:58.559726+00	\N
69e3495d-09f1-4b06-a1d2-28be6de20fa7	ADMIN	93c07834-8c14-4959-b569-7753feeb41aa	\N	t	2026-07-13 14:32:58.559726+00	\N
a4b28b99-3db8-487c-96f1-29abf8ea2e15	ADMIN	8dbf4216-c1f4-4dc7-b359-ba95da111c81	\N	t	2026-07-13 14:32:58.559726+00	\N
c85b518a-feb2-4a60-8188-9e8f54d2a3dd	ADMIN	b0e5ded5-10f6-47ab-99f2-0661df9c3fe6	\N	t	2026-07-13 14:32:58.559726+00	\N
19fab2a9-69eb-4fe9-965a-4f0939de732f	ADMIN	6a40a339-b2f0-487e-bb57-417c9c968942	\N	t	2026-07-13 14:32:58.559726+00	\N
de9df4db-6b6f-40e3-b1ad-ffcd6f399045	ADMIN	360e978a-0e65-4cee-ba98-6db45da12c02	\N	t	2026-07-13 14:32:58.559726+00	\N
cfd032b3-b33a-435b-bebc-ce8e98368b9b	ADMIN	2e5ab04a-4739-47d6-b849-553a46d070b9	\N	t	2026-07-13 14:32:58.559726+00	\N
1702699b-95e5-448e-802a-2e8ff0b343f0	ADMIN	17bc166f-cd4f-4a7d-b3c8-57ca6cd26aeb	\N	t	2026-07-13 14:32:58.559726+00	\N
b408bfb3-9264-462a-804d-4ddaf6730502	ADMIN	85d35dd8-4c5b-47a4-a20a-b97622254ca1	\N	t	2026-07-13 14:32:58.559726+00	\N
9945c892-3eb8-42f2-b733-5814797b94e7	ADMIN	e47f0e7a-f10e-43f9-ac63-ec2e6f5733a2	\N	t	2026-07-13 14:32:58.559726+00	\N
c3718e4f-e239-46f5-88e9-c79c219f647b	ADMIN	3086e507-2971-45f4-a704-e309719b09ba	\N	t	2026-07-13 14:32:58.559726+00	\N
d707acbc-c158-4623-9bbf-428bb54f8eb2	ADMIN	12a401d2-558e-4d4b-92c3-b201ccc33a15	\N	t	2026-07-13 14:32:58.559726+00	\N
4bb7f04f-742c-4e4b-bbbb-39248c4ed948	ADMIN	8bd7c3d8-230a-4810-8000-d43c995d9114	\N	t	2026-07-13 14:32:58.559726+00	\N
74b82f4d-8daa-4493-9f41-c840aa94eb8d	ADMIN	91495fe3-73fc-414e-993b-5efe6e62b395	\N	t	2026-07-13 14:32:58.559726+00	\N
968979c8-0842-4ae2-b110-cd70b2d248c4	ADMIN	9bc711f1-7740-4206-bda2-049f7ca1ce05	\N	t	2026-07-13 14:32:58.559726+00	\N
93142482-bba2-4026-9309-373b276ee7d2	ADMIN	afd619c5-8264-48d4-8004-4b0741005322	\N	t	2026-07-13 14:32:58.559726+00	\N
f4aaea58-7482-40f5-9225-08c803a94aa2	ADMIN	1c2b0209-87f2-40e7-9273-e3019bcf68f8	\N	t	2026-07-13 14:32:58.559726+00	\N
91ab6c3d-8e25-4062-8e03-468873be4a82	ADMIN	aebcf5e2-eec2-402f-bc83-57d463da7891	\N	t	2026-07-13 14:32:58.559726+00	\N
98ad5631-7f4b-4f7c-9b7e-9b2f535748ef	ADMIN	b44a8a35-df1e-40ed-978c-b244bed932c5	\N	t	2026-07-13 14:32:58.559726+00	\N
2912ca48-eb9a-47f7-89aa-ffd341be5d42	ADMIN	49e75bbb-0f2f-4a4b-9af5-a10af4471e31	\N	t	2026-07-13 14:32:58.559726+00	\N
f71b6e13-8358-4586-87ce-305e166ccee4	ADMIN	baf374ab-ef8d-44bb-b10c-1a5ac600d15e	\N	t	2026-07-13 14:32:58.559726+00	\N
0b6329d2-0c7a-48ca-8801-cbba84f0f8a7	ADMIN	fbff25ca-7583-4bc8-92e8-e171296e808c	\N	t	2026-07-13 14:32:58.559726+00	\N
3d43604f-b398-46b1-ae7e-76c56bd8cf35	ADMIN	55aa2940-8396-4410-86ea-073773f3713c	\N	t	2026-07-13 14:32:58.559726+00	\N
4271bfb7-4516-460e-befa-02bad76ae493	ADMIN	23ebd915-4f1e-4293-9e7e-036a88ff6167	\N	t	2026-07-13 14:32:58.559726+00	\N
9b9a3a25-5f31-48fc-8cb5-c23d54640db9	ADMIN	01a16ecc-9b01-4d55-a84d-e9adb1e3a05d	\N	t	2026-07-13 14:32:58.559726+00	\N
25718fe9-c69a-4d9f-999f-d04397e7f864	ADMIN	255418fb-b240-4053-82bf-369e6e972333	\N	t	2026-07-13 14:32:58.559726+00	\N
8353ff54-4adb-4bd2-ad4a-e7877cf875e4	ADMIN	ea8ea4f2-f0b8-4576-b1b1-578e4b3b1994	\N	t	2026-07-13 14:32:58.559726+00	\N
63a094bf-fb63-4143-a32c-49197b845279	SUPERVISOR	8391c42e-12f3-46a8-8cbd-b296377fccb9	\N	t	2026-07-13 14:32:58.724985+00	\N
da5133f8-2c4d-4ff5-adcc-1cb67737ce4f	SUPERVISOR	ebebf206-ea6d-49ed-9bca-0a68b25cc023	\N	t	2026-07-13 14:32:58.724985+00	\N
6b85e39c-a7b8-4d5f-8c59-9c898e4a1331	SUPERVISOR	e06073a6-2fc8-4a0a-b847-8982e59516d2	\N	t	2026-07-13 14:32:58.724985+00	\N
7b02d3e5-c6f0-405e-b42d-a27f7a8e4955	SUPERVISOR	b4f36303-8a3f-4901-9e06-5203f4b47ac6	\N	t	2026-07-13 14:32:58.724985+00	\N
2ce43aef-c19e-4649-ba04-c31a1cdf2101	SUPERVISOR	45f98a89-5d1e-434c-b654-8400c6577531	\N	t	2026-07-13 14:32:58.724985+00	\N
3c4846e5-1f3b-4c80-a62d-37ddce71e4c9	SUPERVISOR	29c43b91-7cec-41de-bc95-d2465c036c2f	\N	t	2026-07-13 14:32:58.724985+00	\N
25112904-604f-4812-9e47-0cb38b4f052f	SUPERVISOR	2bc1f6a3-f800-4d34-af25-2e1d2bbd8387	\N	t	2026-07-13 14:32:58.724985+00	\N
3b102934-eade-4ef5-b304-327264a93bf7	SUPERVISOR	2988aa6d-c1a2-4810-a3e9-531c691b3bbf	\N	t	2026-07-13 14:32:58.724985+00	\N
d6dfda88-4b5b-4507-8520-77388618426c	SUPERVISOR	d4f5ed40-9d3e-420f-8c16-51a9a00e735c	\N	t	2026-07-13 14:32:58.724985+00	\N
ff67714c-650e-4235-a9ec-3a115ad5142b	SUPERVISOR	5a4d012d-236c-4f25-8c90-0459c5497fc0	\N	t	2026-07-13 14:32:58.724985+00	\N
9074b23c-7a86-49b3-8d4a-529c97bf4a7a	SUPERVISOR	2db91016-72a0-453b-bfad-750330a10ea6	\N	t	2026-07-13 14:32:58.724985+00	\N
13f7b850-d098-4c9f-8db7-bde7ce4f43e3	SUPERVISOR	6a5731a3-96e9-4554-a8ca-416244f3ce8c	\N	t	2026-07-13 14:32:58.724985+00	\N
d11b8245-117a-43fe-9b23-99859f314546	SUPERVISOR	4ceb4a31-e27e-47c3-98d7-9ab51a009d9c	\N	t	2026-07-13 14:32:58.724985+00	\N
1d23180b-eb20-4758-b48a-4e9e4f30f883	SUPERVISOR	417cdc41-5943-4196-8023-b98cc62ec54f	\N	t	2026-07-13 14:32:58.724985+00	\N
34a640dc-3179-4ef6-a6f4-25b6764f80cd	SUPERVISOR	d3a62626-0bf3-4a73-9c1b-c371ab648506	\N	t	2026-07-13 14:32:58.724985+00	\N
4e7ae970-3b35-4445-9055-f2eae17f5a1b	SUPERVISOR	0b0a59ac-c417-4492-accc-0cd94890ea15	\N	t	2026-07-13 14:32:58.724985+00	\N
4c339aea-8ce3-4091-9979-8e36a136f20a	SUPERVISOR	42db400c-b1b9-418a-b981-3f598c2ee251	\N	t	2026-07-13 14:32:58.724985+00	\N
2de3aa94-0038-429a-83e3-3914bc8a3668	SUPERVISOR	514f82ee-2153-42ba-a0bd-2f1e2c8a25a2	\N	t	2026-07-13 14:32:58.724985+00	\N
b239fe02-912b-4552-99d1-ef7be4945bc7	SUPERVISOR	5abc681e-b8e1-43f7-ae63-8329a426317b	\N	t	2026-07-13 14:32:58.724985+00	\N
987b1548-f55f-4469-ad6b-2c8c2b69ba70	SUPERVISOR	3d8e1d85-ca77-4629-ba5f-7d21b9c83cd6	\N	t	2026-07-13 14:32:58.724985+00	\N
1308b7cd-6b5c-49ee-9cb1-1cc50a42e8e9	SUPERVISOR	8e1d1e2e-390c-4f21-9fc7-597fda520d01	\N	t	2026-07-13 14:32:58.724985+00	\N
5b2abca2-4979-468c-a54b-9495213d6099	SUPERVISOR	5fd8bf12-dce4-4c43-add4-66d06cbd43c0	\N	t	2026-07-13 14:32:58.724985+00	\N
9b28455f-573d-483c-84da-4f5a7d2b087d	SUPERVISOR	b1543e70-f734-4fb1-bd43-acd9a85d2c66	\N	t	2026-07-13 14:32:58.724985+00	\N
076fa9c3-c09e-4e27-a85e-f69fc328cf3c	SUPERVISOR	6010099c-1920-480a-b700-4410e24b4cff	\N	t	2026-07-13 14:32:58.724985+00	\N
53c9e8bb-e10e-443a-b8f4-8eedb9335507	SUPERVISOR	a8b41d28-2fe9-4eb0-b430-293f3132c6ff	\N	t	2026-07-13 14:32:58.724985+00	\N
aec78003-67ae-4b17-b0dd-4cf65eb6b819	SUPERVISOR	cf71dd83-ad72-43f7-95e7-c723287e63b8	\N	t	2026-07-13 14:32:58.724985+00	\N
1ec801a4-02af-4699-af2d-bdec24a33868	SUPERVISOR	5ef76e7c-d902-4f03-b938-559362b668b4	\N	t	2026-07-13 14:32:58.724985+00	\N
f7b6e6b3-2cb8-4dd0-99f9-50bbc11695c3	SUPERVISOR	0257ab8b-4e70-453f-9a6e-fe784415c562	\N	t	2026-07-13 14:32:58.724985+00	\N
3d3734a0-ba40-49ea-b810-d95d7de18675	SUPERVISOR	2003b1ce-1f70-446d-9915-5efa04f90dcf	\N	t	2026-07-13 14:32:58.724985+00	\N
0d4f0731-2737-4e16-8c26-ed4e097222d5	SUPERVISOR	06fc223e-dc3c-466c-b3cc-fd5aab7b2b08	\N	t	2026-07-13 14:32:58.724985+00	\N
5c62fd5d-1248-4298-aa48-0b6c75e45e45	SUPERVISOR	8c122d4e-371c-47c1-914a-e7f2137765a0	\N	t	2026-07-13 14:32:58.724985+00	\N
638490ab-3f5c-4685-9fcb-d998cef9c3fa	SUPERVISOR	d4e1a5da-39a2-4254-b9d1-c21dbfc784c9	\N	t	2026-07-13 14:32:58.724985+00	\N
320ced5c-43f1-485f-b9ae-4bc2deb3eef6	SUPERVISOR	2600d9da-0099-461a-a6c3-e86a27e5cc34	\N	t	2026-07-13 14:32:58.724985+00	\N
7dadb8f5-099e-437d-b7d2-6b806dbacef4	SUPERVISOR	0d914107-09dc-4fe8-a8be-65f30bcbfeb8	\N	t	2026-07-13 14:32:58.724985+00	\N
b0aad705-2dee-45ee-a5b0-ac6dfe2b0217	SUPERVISOR	93c07834-8c14-4959-b569-7753feeb41aa	\N	t	2026-07-13 14:32:58.724985+00	\N
16506281-da7c-4628-8f7c-c8ae0732bace	SUPERVISOR	8dbf4216-c1f4-4dc7-b359-ba95da111c81	\N	t	2026-07-13 14:32:58.724985+00	\N
d26c0a20-a791-4117-b37a-7d0e0ea079de	SUPERVISOR	b0e5ded5-10f6-47ab-99f2-0661df9c3fe6	\N	t	2026-07-13 14:32:58.724985+00	\N
62ecd114-7493-4186-9c16-cf5af920a3ec	SUPERVISOR	6a40a339-b2f0-487e-bb57-417c9c968942	\N	t	2026-07-13 14:32:58.724985+00	\N
27514536-040f-4939-b716-746fb38c6290	SUPERVISOR	360e978a-0e65-4cee-ba98-6db45da12c02	\N	t	2026-07-13 14:32:58.724985+00	\N
4ff33946-2fa9-4a0c-a4a5-36203e58a8c4	SUPERVISOR	2e5ab04a-4739-47d6-b849-553a46d070b9	\N	t	2026-07-13 14:32:58.724985+00	\N
b03066ed-2f57-486e-8140-47d22c9c86f4	SUPERVISOR	17bc166f-cd4f-4a7d-b3c8-57ca6cd26aeb	\N	t	2026-07-13 14:32:58.724985+00	\N
d155cb7c-e3d6-4d2c-9f86-64df9acdd6c3	SUPERVISOR	85d35dd8-4c5b-47a4-a20a-b97622254ca1	\N	t	2026-07-13 14:32:58.724985+00	\N
d82f2115-2aef-4bb4-9f0b-6a21730fb507	SUPERVISOR	e47f0e7a-f10e-43f9-ac63-ec2e6f5733a2	\N	t	2026-07-13 14:32:58.724985+00	\N
5b8446c4-4465-4c99-9ae2-3a638e86fc67	SUPERVISOR	3086e507-2971-45f4-a704-e309719b09ba	\N	t	2026-07-13 14:32:58.724985+00	\N
d891c5e6-9409-44ec-a0cf-143d9be15d18	SUPERVISOR	12a401d2-558e-4d4b-92c3-b201ccc33a15	\N	t	2026-07-13 14:32:58.724985+00	\N
fc09b333-de9b-47af-b7bd-2e9f1434e28d	SUPERVISOR	8bd7c3d8-230a-4810-8000-d43c995d9114	\N	t	2026-07-13 14:32:58.724985+00	\N
b23d1aaf-82f6-4018-97be-12026fe15267	SUPERVISOR	91495fe3-73fc-414e-993b-5efe6e62b395	\N	t	2026-07-13 14:32:58.724985+00	\N
15abf7b7-fe47-4f86-bbd8-e643c3b7c784	SUPERVISOR	9bc711f1-7740-4206-bda2-049f7ca1ce05	\N	t	2026-07-13 14:32:58.724985+00	\N
16d014bb-761f-4888-b65e-4b2c498fe7c4	SUPERVISOR	afd619c5-8264-48d4-8004-4b0741005322	\N	t	2026-07-13 14:32:58.724985+00	\N
b0529ab0-ee21-4516-ac3b-67dbe5d02a8d	SUPERVISOR	1c2b0209-87f2-40e7-9273-e3019bcf68f8	\N	t	2026-07-13 14:32:58.724985+00	\N
3e15eeee-6b20-49af-8745-39a6cc80ebc6	SUPERVISOR	aebcf5e2-eec2-402f-bc83-57d463da7891	\N	t	2026-07-13 14:32:58.724985+00	\N
1e3548da-6822-4303-9fee-fcb2346c38f3	SUPERVISOR	b44a8a35-df1e-40ed-978c-b244bed932c5	\N	t	2026-07-13 14:32:58.724985+00	\N
f177b26f-21cd-421a-8839-e5c8fbd63321	SUPERVISOR	49e75bbb-0f2f-4a4b-9af5-a10af4471e31	\N	t	2026-07-13 14:32:58.724985+00	\N
2c26af6b-a2aa-4e85-b8c7-1a4f0a188637	SUPERVISOR	baf374ab-ef8d-44bb-b10c-1a5ac600d15e	\N	t	2026-07-13 14:32:58.724985+00	\N
92e44ea3-f540-4c85-a06a-f20bc0a617bf	SUPERVISOR	fbff25ca-7583-4bc8-92e8-e171296e808c	\N	t	2026-07-13 14:32:58.724985+00	\N
54d273b6-0427-49ef-b7c4-f89a87705232	SUPERVISOR	55aa2940-8396-4410-86ea-073773f3713c	\N	t	2026-07-13 14:32:58.724985+00	\N
3ed1f259-13c3-483b-af8d-c4d2b4a67b04	SUPERVISOR	23ebd915-4f1e-4293-9e7e-036a88ff6167	\N	t	2026-07-13 14:32:58.724985+00	\N
e2e3c5e6-5597-4037-a9db-353dc8702bce	SUPERVISOR	01a16ecc-9b01-4d55-a84d-e9adb1e3a05d	\N	t	2026-07-13 14:32:58.724985+00	\N
61a90dfe-1ff1-408c-a838-2c7a5604e887	SUPERVISOR	255418fb-b240-4053-82bf-369e6e972333	\N	t	2026-07-13 14:32:58.724985+00	\N
ab0e475f-ba09-4860-b942-591d4c2b62dd	SUPERVISOR	ea8ea4f2-f0b8-4576-b1b1-578e4b3b1994	\N	t	2026-07-13 14:32:58.724985+00	\N
48f8528f-835b-4d98-b61b-37d3bdd3f667	TECNICO	8391c42e-12f3-46a8-8cbd-b296377fccb9	\N	t	2026-07-13 14:32:58.889548+00	\N
f5156b08-71d3-431f-bf46-977b6ed5a669	TECNICO	2988aa6d-c1a2-4810-a3e9-531c691b3bbf	\N	t	2026-07-13 14:32:58.889548+00	\N
f1e8c236-2744-45c0-aa1d-80aff8b2b831	TECNICO	6a5731a3-96e9-4554-a8ca-416244f3ce8c	\N	t	2026-07-13 14:32:58.889548+00	\N
ba6c7855-fea3-4355-a00b-6b67646554ce	TECNICO	0b0a59ac-c417-4492-accc-0cd94890ea15	\N	t	2026-07-13 14:32:58.889548+00	\N
5f258a5f-3628-467e-ba4e-55cc2ca91644	TECNICO	3d8e1d85-ca77-4629-ba5f-7d21b9c83cd6	\N	t	2026-07-13 14:32:58.889548+00	\N
2a010685-9283-4860-b27b-00b0eccb4c42	TECNICO	6010099c-1920-480a-b700-4410e24b4cff	\N	t	2026-07-13 14:32:58.889548+00	\N
459c3a4b-6333-4ef5-b453-9e37d7291573	TECNICO	0257ab8b-4e70-453f-9a6e-fe784415c562	\N	t	2026-07-13 14:32:58.889548+00	\N
2d923b42-90af-4c67-8ffb-0d7ee910e484	TECNICO	d4e1a5da-39a2-4254-b9d1-c21dbfc784c9	\N	t	2026-07-13 14:32:58.889548+00	\N
35e8ef46-d01f-4e37-8aad-4b125c45b28c	TECNICO	0d914107-09dc-4fe8-a8be-65f30bcbfeb8	\N	t	2026-07-13 14:32:58.889548+00	\N
81daa8f0-f957-4385-b02a-4503f1527544	TECNICO	b0e5ded5-10f6-47ab-99f2-0661df9c3fe6	\N	t	2026-07-13 14:32:58.889548+00	\N
182d32a8-7cc3-4591-9da9-5df0dd20156f	TECNICO	17bc166f-cd4f-4a7d-b3c8-57ca6cd26aeb	\N	t	2026-07-13 14:32:58.889548+00	\N
1fcdaaf9-9c52-4942-9725-d3c8e633bdc8	TECNICO	85d35dd8-4c5b-47a4-a20a-b97622254ca1	\N	t	2026-07-13 14:32:58.889548+00	\N
b1b85edf-01c5-47d0-8259-7e8ea8f61ee9	TECNICO	e47f0e7a-f10e-43f9-ac63-ec2e6f5733a2	\N	t	2026-07-13 14:32:58.889548+00	\N
7ea25076-4c61-4a6c-bb2b-b494a7e32fa3	TECNICO	3086e507-2971-45f4-a704-e309719b09ba	\N	t	2026-07-13 14:32:58.889548+00	\N
e18ef4f4-512c-4d4b-8160-8d01fbc9d58f	TECNICO	aebcf5e2-eec2-402f-bc83-57d463da7891	\N	t	2026-07-13 14:32:58.889548+00	\N
3b4fe517-a489-4730-87d3-5357f3af0678	TECNICO	b44a8a35-df1e-40ed-978c-b244bed932c5	\N	t	2026-07-13 14:32:58.889548+00	\N
9c65f79a-64dd-4ac3-abd3-ed92d158e538	TECNICO	49e75bbb-0f2f-4a4b-9af5-a10af4471e31	\N	t	2026-07-13 14:32:58.889548+00	\N
ed484e1f-ab9b-43d2-80b6-288145b867be	TECNICO	baf374ab-ef8d-44bb-b10c-1a5ac600d15e	\N	t	2026-07-13 14:32:58.889548+00	\N
463e1bca-02a1-421a-8ae0-e547c33fcc2b	TECNICO	fbff25ca-7583-4bc8-92e8-e171296e808c	\N	t	2026-07-13 14:32:58.889548+00	\N
e8bff802-837a-438e-b287-a76eefa84f61	TRABAJADOR	2988aa6d-c1a2-4810-a3e9-531c691b3bbf	\N	t	2026-07-13 14:32:59.052074+00	\N
f0069bd3-0fbd-42e6-b280-9011e24fc906	TRABAJADOR	6a5731a3-96e9-4554-a8ca-416244f3ce8c	\N	t	2026-07-13 14:32:59.052074+00	\N
5d0472d1-5b9a-43fa-b3da-be98e1ed6bec	TRABAJADOR	0b0a59ac-c417-4492-accc-0cd94890ea15	\N	t	2026-07-13 14:32:59.052074+00	\N
f1eac7a5-f691-427e-8fae-b10a41f16711	TRABAJADOR	3d8e1d85-ca77-4629-ba5f-7d21b9c83cd6	\N	t	2026-07-13 14:32:59.052074+00	\N
5addf1c4-c985-43b1-a403-65372c7875d2	TRABAJADOR	d4e1a5da-39a2-4254-b9d1-c21dbfc784c9	\N	t	2026-07-13 14:32:59.052074+00	\N
b1e3aa93-9c9e-41ef-9f52-85326f5bd17b	TRABAJADOR	0d914107-09dc-4fe8-a8be-65f30bcbfeb8	\N	t	2026-07-13 14:32:59.052074+00	\N
c6c3966f-29a5-4fdf-abd9-2fd29469af74	TRABAJADOR	8dbf4216-c1f4-4dc7-b359-ba95da111c81	\N	t	2026-07-13 14:32:59.052074+00	\N
cbd791d8-0111-4472-8e57-8458fe49af6c	TRABAJADOR	b0e5ded5-10f6-47ab-99f2-0661df9c3fe6	\N	t	2026-07-13 14:32:59.052074+00	\N
6db8f977-1fcd-466d-8fc7-e229b7c3f51e	TRABAJADOR	12a401d2-558e-4d4b-92c3-b201ccc33a15	\N	t	2026-07-13 14:32:59.052074+00	\N
c4551c55-4137-42c1-b4ea-65911bdad792	TRABAJADOR	8bd7c3d8-230a-4810-8000-d43c995d9114	\N	t	2026-07-13 14:32:59.052074+00	\N
13ac4e7c-334e-40cd-8848-548f2a3e38b6	TRABAJADOR	91495fe3-73fc-414e-993b-5efe6e62b395	\N	t	2026-07-13 14:32:59.052074+00	\N
8cc92082-9351-49a2-b5f6-4a26ce51aebf	TRABAJADOR	1c2b0209-87f2-40e7-9273-e3019bcf68f8	\N	t	2026-07-13 14:32:59.052074+00	\N
7b809672-cd23-4187-90e5-68163a6b93c3	TRABAJADOR	aebcf5e2-eec2-402f-bc83-57d463da7891	\N	t	2026-07-13 14:32:59.052074+00	\N
4739ba61-488c-403a-9b6c-29c368262ada	TRABAJADOR	b44a8a35-df1e-40ed-978c-b244bed932c5	\N	t	2026-07-13 14:32:59.052074+00	\N
8bff9ada-a5d5-4232-a2f6-61564018213c	TRABAJADOR	49e75bbb-0f2f-4a4b-9af5-a10af4471e31	\N	t	2026-07-13 14:32:59.052074+00	\N
246aafb3-4162-4d85-b54c-5eb5a10f6ce0	TRABAJADOR	baf374ab-ef8d-44bb-b10c-1a5ac600d15e	\N	t	2026-07-13 14:32:59.052074+00	\N
da9e3532-0cff-49e8-8338-ce756a7b0fba	TRABAJADOR	fbff25ca-7583-4bc8-92e8-e171296e808c	\N	t	2026-07-13 14:32:59.052074+00	\N
c210ccd1-907c-4b98-b60b-c457c765fb01	USUARIO	6a5731a3-96e9-4554-a8ca-416244f3ce8c	\N	t	2026-07-13 14:32:59.214549+00	\N
842ce32d-5f62-4549-be3b-9a5ad4974a23	USUARIO	0b0a59ac-c417-4492-accc-0cd94890ea15	\N	t	2026-07-13 14:32:59.214549+00	\N
31cabc7d-9930-4078-8943-0a3ee75fd775	USUARIO	3d8e1d85-ca77-4629-ba5f-7d21b9c83cd6	\N	t	2026-07-13 14:32:59.214549+00	\N
e9ff2617-5cd4-4d97-a2a1-d7e9312f5323	USUARIO	d4e1a5da-39a2-4254-b9d1-c21dbfc784c9	\N	t	2026-07-13 14:32:59.214549+00	\N
6ae1b8d2-b3b9-44cb-86e9-07c30160e033	USUARIO	0d914107-09dc-4fe8-a8be-65f30bcbfeb8	\N	t	2026-07-13 14:32:59.214549+00	\N
77384e83-0303-4c1a-9693-55849cb22629	USUARIO	8dbf4216-c1f4-4dc7-b359-ba95da111c81	\N	t	2026-07-13 14:32:59.214549+00	\N
78c951ff-8124-4d38-af21-8890eb20c554	USUARIO	b0e5ded5-10f6-47ab-99f2-0661df9c3fe6	\N	t	2026-07-13 14:32:59.214549+00	\N
0eef1e2e-06e3-4939-a7fe-81472694530d	USUARIO	12a401d2-558e-4d4b-92c3-b201ccc33a15	\N	t	2026-07-13 14:32:59.214549+00	\N
0a5b1deb-9a11-47ce-8881-650e40bb44af	USUARIO	8bd7c3d8-230a-4810-8000-d43c995d9114	\N	t	2026-07-13 14:32:59.214549+00	\N
66c89101-74db-4a22-8b18-5ab27fc09767	USUARIO	91495fe3-73fc-414e-993b-5efe6e62b395	\N	t	2026-07-13 14:32:59.214549+00	\N
bfc7e704-4730-449f-bd59-8e2c5b2a3d23	USUARIO	1c2b0209-87f2-40e7-9273-e3019bcf68f8	\N	t	2026-07-13 14:32:59.214549+00	\N
ad4148cc-be35-447a-b88b-72fa21b1176c	USUARIO	aebcf5e2-eec2-402f-bc83-57d463da7891	\N	t	2026-07-13 14:32:59.214549+00	\N
063112b7-f3ef-4606-a7b9-bdafbc12fecc	USUARIO	49e75bbb-0f2f-4a4b-9af5-a10af4471e31	\N	t	2026-07-13 14:32:59.214549+00	\N
fb77a9e4-3302-455b-9e3d-e152ba74115e	USUARIO	baf374ab-ef8d-44bb-b10c-1a5ac600d15e	\N	t	2026-07-13 14:32:59.214549+00	\N
730d870c-4ed2-442c-ae00-1ddb8f46fa05	USUARIO	fbff25ca-7583-4bc8-92e8-e171296e808c	\N	t	2026-07-13 14:32:59.214549+00	\N
\.


ALTER TABLE public.role_permissions ENABLE TRIGGER ALL;

--
-- Data for Name: tecnico_sucursales; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.tecnico_sucursales DISABLE TRIGGER ALL;

COPY public.tecnico_sucursales (id, tecnico_id, sucursal_id, es_principal, activa, created_at, updated_at, created_by, updated_by) FROM stdin;
\.


ALTER TABLE public.tecnico_sucursales ENABLE TRIGGER ALL;

--
-- Data for Name: ticket_asignaciones; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.ticket_asignaciones DISABLE TRIGGER ALL;

COPY public.ticket_asignaciones (id, ticket_id, tecnico_id, asignador_id, es_reasignacion, tecnico_anterior_id, motivo_reasignacion, created_at, created_by) FROM stdin;
2b9db9ac-de3a-4b8b-a976-0eafe7fcb0aa	e44d4b6c-f9de-4b10-926d-8eb418d445a5	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	21540103-61cc-4141-a3f1-11763957b648	f	\N	\N	2026-07-17 21:13:09.945959+00	21540103-61cc-4141-a3f1-11763957b648
4503a065-b376-4998-b034-b1b8597c5353	619711dc-e907-4b64-bf75-d64e657a2790	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	ffced565-d714-42ce-a4f1-995c9511441c	f	\N	\N	2026-07-21 14:30:50.285846+00	ffced565-d714-42ce-a4f1-995c9511441c
5da6b4cc-c3a3-4a17-9d67-b92aefcab698	fb23048f-b94a-41ba-9353-f1754f02fb61	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	21540103-61cc-4141-a3f1-11763957b648	f	\N	\N	2026-07-31 01:35:03.474608+00	21540103-61cc-4141-a3f1-11763957b648
d972448e-8957-46f5-9d34-f6c130ed43e2	487f9484-9b26-42b8-8439-395d9625fe19	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	ffced565-d714-42ce-a4f1-995c9511441c	f	\N	\N	2026-08-03 14:53:35.30754+00	ffced565-d714-42ce-a4f1-995c9511441c
50d3f753-f60a-473b-bcec-b118854bd198	c4a88eb1-027c-407d-b024-e9ad07a17bd1	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	ffced565-d714-42ce-a4f1-995c9511441c	f	\N	\N	2026-08-03 16:19:14.207868+00	ffced565-d714-42ce-a4f1-995c9511441c
db85a4b2-707d-4a7d-800f-61a5bc3353ed	d9cb455e-d34a-4ef1-bf19-14068044a85a	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	ffced565-d714-42ce-a4f1-995c9511441c	f	\N	\N	2026-08-04 16:09:19.581606+00	ffced565-d714-42ce-a4f1-995c9511441c
6e83ec25-5c8c-4764-be6a-1be9cac60fba	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	ffced565-d714-42ce-a4f1-995c9511441c	f	\N	\N	2026-08-04 17:07:46.330568+00	ffced565-d714-42ce-a4f1-995c9511441c
ad5d5e75-4846-4c05-b69c-1b3c5c4b1c79	c4a88eb1-027c-407d-b024-e9ad07a17bd1	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	21540103-61cc-4141-a3f1-11763957b648	f	\N	\N	2026-08-06 19:50:37.123046+00	21540103-61cc-4141-a3f1-11763957b648
95a0923d-f3a6-4c99-8619-dc231345777d	62b633a8-4a8b-4bca-9d00-8f098c6b2452	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	21540103-61cc-4141-a3f1-11763957b648	f	\N	\N	2026-08-06 19:51:26.62054+00	21540103-61cc-4141-a3f1-11763957b648
2bd6d2e7-6d9c-4d43-a944-3b28e62f67a0	f856d580-0907-4f8f-ae86-7007cf8d527b	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	ffced565-d714-42ce-a4f1-995c9511441c	f	\N	\N	2026-08-12 13:44:03.738894+00	ffced565-d714-42ce-a4f1-995c9511441c
cadb3d7f-4801-487b-b99b-f395c5b46330	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	ffced565-d714-42ce-a4f1-995c9511441c	f	\N	\N	2026-08-12 14:27:30.914104+00	ffced565-d714-42ce-a4f1-995c9511441c
16543c55-94e1-4fa6-97fd-31a27d7ab94a	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	ffced565-d714-42ce-a4f1-995c9511441c	f	\N	\N	2026-08-18 17:51:20.631464+00	ffced565-d714-42ce-a4f1-995c9511441c
8f1072f4-7ec0-4984-aefd-94cf58c3ef60	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	ffced565-d714-42ce-a4f1-995c9511441c	f	\N	\N	2026-08-18 19:51:16.832233+00	ffced565-d714-42ce-a4f1-995c9511441c
\.


ALTER TABLE public.ticket_asignaciones ENABLE TRIGGER ALL;

--
-- Data for Name: ticket_comentarios; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.ticket_comentarios DISABLE TRIGGER ALL;

COPY public.ticket_comentarios (id, ticket_id, autor_id, cuerpo, es_interno, editado_en, created_at, updated_by, created_by, deleted_at, deleted_by) FROM stdin;
db492da6-2228-4d9a-b8e0-e28e0c3578e7	6d09f602-356f-457c-ba23-e65000d73c22	21540103-61cc-4141-a3f1-11763957b648	Hola como va el ticket?	f	\N	2026-07-17 19:46:11.97294+00	\N	21540103-61cc-4141-a3f1-11763957b648	\N	\N
ecccf086-94a1-4377-a1f8-867b8bc8e655	e44d4b6c-f9de-4b10-926d-8eb418d445a5	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	gracias por el ticket	f	\N	2026-07-17 21:14:12.270603+00	\N	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	\N	\N
e4beb43e-7d07-43b5-9b20-e59dfa4d134a	487f9484-9b26-42b8-8439-395d9625fe19	ffced565-d714-42ce-a4f1-995c9511441c	soporte prueba	f	\N	2026-08-03 14:51:05.301543+00	\N	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
f4118cb8-1e96-4b09-a559-acc7cc52397c	c4a88eb1-027c-407d-b024-e9ad07a17bd1	ffced565-d714-42ce-a4f1-995c9511441c	nuevo ticket creado1	f	\N	2026-08-03 16:17:59.035162+00	\N	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
8b9badeb-82cb-404d-ab23-2dc43ab7fd5a	c4a88eb1-027c-407d-b024-e9ad07a17bd1	ffced565-d714-42ce-a4f1-995c9511441c	ticket nuevo 1	f	\N	2026-08-03 16:22:29.389016+00	\N	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
7ea69f41-e335-4bc4-9495-cd487f86e5e9	c4a88eb1-027c-407d-b024-e9ad07a17bd1	ffced565-d714-42ce-a4f1-995c9511441c	aprobado	f	\N	2026-08-03 17:54:21.831439+00	\N	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
02411b8b-59bd-4a94-91a0-4416285d702d	c4a88eb1-027c-407d-b024-e9ad07a17bd1	25541c55-aafe-4714-98d1-a177b057302e	aprobado	f	\N	2026-08-03 17:54:54.738041+00	\N	25541c55-aafe-4714-98d1-a177b057302e	\N	\N
f291a190-55b5-4b80-8b58-a25e1fd8f876	c4a88eb1-027c-407d-b024-e9ad07a17bd1	ffced565-d714-42ce-a4f1-995c9511441c	.	f	\N	2026-08-03 20:38:57.358624+00	\N	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
3d86bb4e-8edc-45bf-a538-d7eeb493feb2	26bb057a-9c13-436f-9d2b-1178aa1ad034	25541c55-aafe-4714-98d1-a177b057302e	se va cancelar el ticket porque ya no se necesita soporte , fue solucionado internamente	f	\N	2026-08-03 21:04:37.47143+00	\N	25541c55-aafe-4714-98d1-a177b057302e	\N	\N
5eaded82-a64c-430d-965d-7bcf3cde9007	d9cb455e-d34a-4ef1-bf19-14068044a85a	ffced565-d714-42ce-a4f1-995c9511441c	creacion de nuevo ticket	f	\N	2026-08-04 16:09:04.994573+00	\N	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
22acd005-c564-4d2f-8805-a9a1b38ee4c9	d9cb455e-d34a-4ef1-bf19-14068044a85a	ffced565-d714-42ce-a4f1-995c9511441c	prueba	f	\N	2026-08-04 16:16:41.780987+00	\N	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
dad2cd2a-e06a-4d8b-8fed-e1a29725e4b8	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	ticket de prueba	f	\N	2026-08-04 17:15:04.608387+00	\N	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	\N	\N
184e3856-fa38-4c0e-8a5d-fd7d8c5767f4	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	.	f	\N	2026-08-04 17:20:09.207229+00	\N	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	\N	\N
6806185f-eb39-40d1-8dd7-b16a921c7b31	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	ffced565-d714-42ce-a4f1-995c9511441c	.	f	\N	2026-08-04 17:31:53.530122+00	\N	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
db1d768e-6beb-456b-8b9b-3f63dc30d8bf	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	ffced565-d714-42ce-a4f1-995c9511441c	no deja cerrar el ticket	f	\N	2026-08-04 19:25:42.089308+00	\N	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
1adff4db-fd87-4bf1-87cf-fe6b906872eb	f856d580-0907-4f8f-ae86-7007cf8d527b	ffced565-d714-42ce-a4f1-995c9511441c	hola	f	\N	2026-08-12 13:43:54.232829+00	\N	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
38355522-3d28-4516-92f3-105c0c665326	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	hola	f	\N	2026-08-12 14:29:15.939186+00	\N	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
7330c4e9-29e5-4b7f-be7f-7bc1d01ed612	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	hola	f	\N	2026-08-12 14:30:56.433638+00	\N	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
6b1b6d78-7742-4c4e-9633-4028d0f35103	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	f	f	\N	2026-08-17 14:58:45.468458+00	\N	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
97d8250e-ebf4-4e7d-8d02-b268b6891d60	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	ffced565-d714-42ce-a4f1-995c9511441c	Hola	f	\N	2026-08-18 17:43:52.333304+00	\N	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
78d8165f-1333-47d1-bbcb-7238e71f20c0	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	ffced565-d714-42ce-a4f1-995c9511441c	hola .	f	\N	2026-08-18 19:45:40.507639+00	\N	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
\.


ALTER TABLE public.ticket_comentarios ENABLE TRIGGER ALL;

--
-- Data for Name: ticket_evidencias; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.ticket_evidencias DISABLE TRIGGER ALL;

COPY public.ticket_evidencias (id, ticket_id, autor_id, tipo, nombre_original, tipo_mime, tamano_bytes, url_almacenamiento, created_at, created_by, deleted_at, deleted_by) FROM stdin;
6ace13e5-e76f-4ef5-baca-37b2a32db49c	6d09f602-356f-457c-ba23-e65000d73c22	21540103-61cc-4141-a3f1-11763957b648	INICIAL	evidencia.jpg	image/jpeg	32954	tickets/6d09f602-356f-457c-ba23-e65000d73c22/1ce59097-ed7d-4120-965d-7cc49a448905-evidencia.jpg	2026-07-17 19:46:31.261329+00	21540103-61cc-4141-a3f1-11763957b648	\N	\N
7c3ffbe4-4330-4adc-b020-62d9194d029a	e44d4b6c-f9de-4b10-926d-8eb418d445a5	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	INICIAL	evidencia-arreglo.jpg	image/jpeg	21976	tickets/e44d4b6c-f9de-4b10-926d-8eb418d445a5/00f2254b-57e1-4768-90ae-45d229536dcd-evidencia-arreglo.jpg	2026-07-17 21:14:27.189849+00	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	\N	\N
e7a3e3cc-7cf4-4206-9e71-f528e8fbf373	e6bc53e4-6fd0-4965-b7c5-bcf79185bb05	21540103-61cc-4141-a3f1-11763957b648	INICIAL	evidencia.jpg	image/jpeg	32954	tickets/e6bc53e4-6fd0-4965-b7c5-bcf79185bb05/fcdc9d43-7fd1-4bb3-9945-c717b0b1169c-evidencia.jpg	2026-07-20 20:32:43.990981+00	21540103-61cc-4141-a3f1-11763957b648	\N	\N
6fddf6c8-4737-4491-95be-fcc1d52a4d63	619711dc-e907-4b64-bf75-d64e657a2790	ffced565-d714-42ce-a4f1-995c9511441c	INICIAL	error.jpg	image/jpeg	94458	tickets/619711dc-e907-4b64-bf75-d64e657a2790/8026900c-d294-459b-a3c0-783c3c43c6d5-error.jpg	2026-07-21 14:29:51.524842+00	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
a31f2b66-e81a-42ad-a02f-325bb17e464d	487f9484-9b26-42b8-8439-395d9625fe19	ffced565-d714-42ce-a4f1-995c9511441c	INICIAL	Captura de pantalla 2026-07-21 130144.png	image/png	65439	tickets/487f9484-9b26-42b8-8439-395d9625fe19/3371c009-4423-4cb4-8b49-9d8eddf4a6c0-Captura de pantalla 2026-07-21 130144.png	2026-08-03 14:52:04.626741+00	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
d2d083ec-31c4-4f56-83f6-53da17b413cb	c4a88eb1-027c-407d-b024-e9ad07a17bd1	25541c55-aafe-4714-98d1-a177b057302e	INICIAL	Captura de pantalla 2026-07-24 175507.png	image/png	148644	tickets/c4a88eb1-027c-407d-b024-e9ad07a17bd1/7b42dbbe-2561-4914-984c-eea025540403-Captura de pantalla 2026-07-24 175507.png	2026-08-03 16:16:02.570677+00	25541c55-aafe-4714-98d1-a177b057302e	\N	\N
6d06599e-158f-449b-87bd-d2a0ec95353f	e3842e17-5cfc-4cbb-a504-010a3293a55a	25541c55-aafe-4714-98d1-a177b057302e	INICIAL	Captura de pantalla 2026-07-21 124951.png	image/png	2642	tickets/e3842e17-5cfc-4cbb-a504-010a3293a55a/c140fa79-b7bc-4e2d-b3ba-c845495445e5-Captura de pantalla 2026-07-21 124951.png	2026-08-03 20:50:30.844919+00	25541c55-aafe-4714-98d1-a177b057302e	\N	\N
9af89018-2e95-4858-829e-b50ad3619b20	e3842e17-5cfc-4cbb-a504-010a3293a55a	25541c55-aafe-4714-98d1-a177b057302e	INICIAL	Captura de pantalla 2026-07-21 125947.png	image/png	104455	tickets/e3842e17-5cfc-4cbb-a504-010a3293a55a/1c31181d-3b8e-41d1-a586-f317ff9c0cc1-Captura de pantalla 2026-07-21 125947.png	2026-08-03 20:50:31.507226+00	25541c55-aafe-4714-98d1-a177b057302e	\N	\N
6d07f018-3984-486b-beb6-8d784ebcaec1	26bb057a-9c13-436f-9d2b-1178aa1ad034	25541c55-aafe-4714-98d1-a177b057302e	INICIAL	Captura de pantalla 2026-07-24 143411.png	image/png	122741	tickets/26bb057a-9c13-436f-9d2b-1178aa1ad034/bc80f795-7c0e-4383-86ac-2caec083af23-Captura de pantalla 2026-07-24 143411.png	2026-08-03 21:00:49.247352+00	25541c55-aafe-4714-98d1-a177b057302e	\N	\N
a017d630-a880-4e38-a222-2b158a378296	d9cb455e-d34a-4ef1-bf19-14068044a85a	ffced565-d714-42ce-a4f1-995c9511441c	INICIAL	Captura de pantalla 2026-07-24 143121.png	image/png	162056	tickets/d9cb455e-d34a-4ef1-bf19-14068044a85a/0b30047c-b738-4a84-a8f7-dbdba19365cf-Captura de pantalla 2026-07-24 143121.png	2026-08-04 16:04:30.331866+00	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
98874d41-2aab-44a1-a32d-3ae8380bc136	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	INICIAL	Captura de pantalla 2026-07-21 125947.png	image/png	104455	tickets/bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400/98a73415-de54-4ba9-935c-36cabec1ae8d-Captura de pantalla 2026-07-21 125947.png	2026-08-04 17:07:11.312695+00	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	\N	\N
9d55aef1-ae57-4ac4-8b52-0c5d5205d1d6	f856d580-0907-4f8f-ae86-7007cf8d527b	ffced565-d714-42ce-a4f1-995c9511441c	INICIAL	Captura de pantalla 2026-07-24 143411.png	image/png	122741	tickets/f856d580-0907-4f8f-ae86-7007cf8d527b/4ea83cd8-c928-4ae2-80a3-ad52c75c3f1e-Captura de pantalla 2026-07-24 143411.png	2026-08-12 13:42:14.306714+00	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
45ee1657-96d4-4963-9fda-c6e73f19ffd7	f856d580-0907-4f8f-ae86-7007cf8d527b	ffced565-d714-42ce-a4f1-995c9511441c	INICIAL	Captura de pantalla 2026-07-27 122032.png	image/png	163402	tickets/f856d580-0907-4f8f-ae86-7007cf8d527b/1180940d-100a-4025-998e-a2cfa7765e75-Captura de pantalla 2026-07-27 122032.png	2026-08-12 13:42:14.320261+00	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
4518976d-bbd2-41c8-a486-c5e6e48eea23	f856d580-0907-4f8f-ae86-7007cf8d527b	ffced565-d714-42ce-a4f1-995c9511441c	INICIAL	Captura de pantalla 2026-07-21 125947.png	image/png	104455	tickets/f856d580-0907-4f8f-ae86-7007cf8d527b/cde5eb79-91a2-43a9-b8b5-095615af7310-Captura de pantalla 2026-07-21 125947.png	2026-08-12 13:42:14.326557+00	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
df9a323d-4c6b-4739-8465-a37cafd77f56	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	INICIAL	Captura de pantalla 2026-07-21 130144.png	image/png	65439	tickets/9b008df6-460a-4b34-b6cb-1d1ed878f6ab/c3365293-000e-47da-bd66-93513fe3f2ac-Captura de pantalla 2026-07-21 130144.png	2026-08-12 14:26:57.110674+00	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
a33f158b-4c41-411e-9c4c-839955d37a7e	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	INICIAL	Captura de pantalla 2026-07-21 124553.png	image/png	117106	tickets/9b008df6-460a-4b34-b6cb-1d1ed878f6ab/1c6cd278-4570-476a-9987-d37635adbb89-Captura de pantalla 2026-07-21 124553.png	2026-08-12 14:26:57.112284+00	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
7c3ed661-1785-4d1c-8071-7441bde2cbb0	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	ffced565-d714-42ce-a4f1-995c9511441c	INICIAL	Captura de pantalla 2026-07-21 180041.png	image/png	162710	tickets/f2de07d3-d7c2-4ba3-9691-5c12bc13b23a/16636105-fcfa-490f-b871-4404bdf3ec21-Captura de pantalla 2026-07-21 180041.png	2026-08-18 17:42:58.432983+00	ffced565-d714-42ce-a4f1-995c9511441c	\N	\N
\.


ALTER TABLE public.ticket_evidencias ENABLE TRIGGER ALL;

--
-- Data for Name: ticket_historial; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.ticket_historial DISABLE TRIGGER ALL;

COPY public.ticket_historial (id, ticket_id, actor_id, tipo_evento, estado_anterior, estado_nuevo, comentario_texto, rejection_reason_id, rejection_comment, metadata, created_at, created_by) FROM stdin;
251e89ac-5340-4f8f-97e1-762ea2662a68	6d09f602-356f-457c-ba23-e65000d73c22	21540103-61cc-4141-a3f1-11763957b648	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-07-17 19:03:50.546189+00	21540103-61cc-4141-a3f1-11763957b648
ed12f58a-3ed2-4e45-af91-54596a536739	6d09f602-356f-457c-ba23-e65000d73c22	21540103-61cc-4141-a3f1-11763957b648	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-07-17 19:03:50.581322+00	21540103-61cc-4141-a3f1-11763957b648
cf4f2dbd-2186-49a6-9e9e-d1d3ca65c399	6d09f602-356f-457c-ba23-e65000d73c22	21540103-61cc-4141-a3f1-11763957b648	COMENTADO	\N	\N	Hola como va el ticket?	\N	\N	\N	2026-07-17 19:46:12.062751+00	21540103-61cc-4141-a3f1-11763957b648
1ed64857-272d-4cc3-9637-6e7f9d3de38e	6d09f602-356f-457c-ba23-e65000d73c22	21540103-61cc-4141-a3f1-11763957b648	EVIDENCIA_SUBIDA	\N	\N	\N	\N	\N	\N	2026-07-17 19:46:31.353217+00	21540103-61cc-4141-a3f1-11763957b648
c2aa929c-5b70-46a4-a40b-926917b45266	496824d7-63ad-427b-b648-3357ccd20b83	21540103-61cc-4141-a3f1-11763957b648	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-07-17 20:08:36.66323+00	21540103-61cc-4141-a3f1-11763957b648
8c8a805e-96f9-4267-a555-dace18265e67	496824d7-63ad-427b-b648-3357ccd20b83	21540103-61cc-4141-a3f1-11763957b648	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-07-17 20:08:36.746609+00	21540103-61cc-4141-a3f1-11763957b648
0853a442-6bf0-41ff-869c-87b1e99df470	496824d7-63ad-427b-b648-3357ccd20b83	21540103-61cc-4141-a3f1-11763957b648	PRIORIDAD_CAMBIADA	\N	\N	\N	\N	\N	{"nueva": "MEDIA", "anterior": "ALTA"}	2026-07-17 20:13:07.661069+00	21540103-61cc-4141-a3f1-11763957b648
d5545d99-ea16-4970-a02f-a2364bf19a16	496824d7-63ad-427b-b648-3357ccd20b83	21540103-61cc-4141-a3f1-11763957b648	PRIORIDAD_CAMBIADA	\N	\N	\N	\N	\N	{"nueva": "CRITICA", "anterior": "MEDIA"}	2026-07-17 20:13:26.469593+00	21540103-61cc-4141-a3f1-11763957b648
2227108b-09eb-4f31-a1e6-ba65f30f9068	e44d4b6c-f9de-4b10-926d-8eb418d445a5	02419c75-3006-4f51-8019-a435201f52ba	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-07-17 21:10:22.099067+00	02419c75-3006-4f51-8019-a435201f52ba
44c59da4-71af-4b92-9b6b-e385eb202228	e44d4b6c-f9de-4b10-926d-8eb418d445a5	02419c75-3006-4f51-8019-a435201f52ba	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-07-17 21:10:22.146299+00	02419c75-3006-4f51-8019-a435201f52ba
c0502863-dc97-4d25-a534-6096df6378a5	e44d4b6c-f9de-4b10-926d-8eb418d445a5	21540103-61cc-4141-a3f1-11763957b648	ASIGNADO	SIN_ASIGNAR	ASIGNADO	\N	\N	\N	\N	2026-07-17 21:13:09.923545+00	21540103-61cc-4141-a3f1-11763957b648
714afb46-072a-422e-a4f7-0e04ae962803	e44d4b6c-f9de-4b10-926d-8eb418d445a5	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	COMENTADO	\N	\N	gracias por el ticket	\N	\N	\N	2026-07-17 21:14:12.334174+00	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea
a93f2a8d-8d98-4816-93d5-25911c1cd153	e44d4b6c-f9de-4b10-926d-8eb418d445a5	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	EVIDENCIA_SUBIDA	\N	\N	\N	\N	\N	\N	2026-07-17 21:14:27.218266+00	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea
ca0bccc0-eb3e-4024-880a-5cdb07db1374	e44d4b6c-f9de-4b10-926d-8eb418d445a5	21540103-61cc-4141-a3f1-11763957b648	ESTADO_CAMBIADO	ASIGNADO	EN_PROCESO	\N	\N	\N	\N	2026-07-17 21:15:15.824131+00	21540103-61cc-4141-a3f1-11763957b648
d1b53031-2948-45e3-ad9c-08ac8b718212	62b633a8-4a8b-4bca-9d00-8f098c6b2452	21540103-61cc-4141-a3f1-11763957b648	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-07-18 18:14:23.949004+00	21540103-61cc-4141-a3f1-11763957b648
c3bab89f-098f-41ee-acf9-30ebbe47a3d9	62b633a8-4a8b-4bca-9d00-8f098c6b2452	21540103-61cc-4141-a3f1-11763957b648	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-07-18 18:14:23.989856+00	21540103-61cc-4141-a3f1-11763957b648
ebd4e511-99bf-4966-9d19-8aea1f99bdd9	e6bc53e4-6fd0-4965-b7c5-bcf79185bb05	21540103-61cc-4141-a3f1-11763957b648	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-07-20 20:32:41.235676+00	21540103-61cc-4141-a3f1-11763957b648
9fa5628e-f386-46a4-8aab-6ee6cdc3dd89	e6bc53e4-6fd0-4965-b7c5-bcf79185bb05	21540103-61cc-4141-a3f1-11763957b648	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-07-20 20:32:41.26312+00	21540103-61cc-4141-a3f1-11763957b648
65ebe38e-490b-41cb-812d-8461dbd44cf3	e6bc53e4-6fd0-4965-b7c5-bcf79185bb05	21540103-61cc-4141-a3f1-11763957b648	EVIDENCIA_SUBIDA	\N	\N	\N	\N	\N	\N	2026-07-20 20:32:44.036311+00	21540103-61cc-4141-a3f1-11763957b648
3f14a57c-99ea-4612-827a-a6d27ed40195	8ba5f1be-4ce8-434d-be41-3f6edcc59e4f	21540103-61cc-4141-a3f1-11763957b648	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-07-20 22:02:03.934878+00	21540103-61cc-4141-a3f1-11763957b648
a11919bf-c7e0-455d-98f8-55dc6bcb293c	8ba5f1be-4ce8-434d-be41-3f6edcc59e4f	21540103-61cc-4141-a3f1-11763957b648	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-07-20 22:02:03.983747+00	21540103-61cc-4141-a3f1-11763957b648
5b110743-819f-4cd0-bea6-66cc7dafed84	8ba5f1be-4ce8-434d-be41-3f6edcc59e4f	21540103-61cc-4141-a3f1-11763957b648	DATOS_ACTUALIZADOS	\N	\N	\N	\N	\N	{"tipoAnterior": "641ca311-3280-4faa-ac49-285822a16f02", "tituloAnterior": ""}	2026-07-20 22:08:28.943536+00	21540103-61cc-4141-a3f1-11763957b648
f99bd5c0-ddf4-4384-80bd-7c63415bb325	619711dc-e907-4b64-bf75-d64e657a2790	ffced565-d714-42ce-a4f1-995c9511441c	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-07-21 14:29:48.866893+00	ffced565-d714-42ce-a4f1-995c9511441c
a3da3264-e444-467b-bb28-5b556191ce5d	619711dc-e907-4b64-bf75-d64e657a2790	ffced565-d714-42ce-a4f1-995c9511441c	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-07-21 14:29:48.901843+00	ffced565-d714-42ce-a4f1-995c9511441c
6ada202a-cda9-4d74-9fa8-69ad91cb627e	619711dc-e907-4b64-bf75-d64e657a2790	ffced565-d714-42ce-a4f1-995c9511441c	EVIDENCIA_SUBIDA	\N	\N	\N	\N	\N	\N	2026-07-21 14:29:51.560299+00	ffced565-d714-42ce-a4f1-995c9511441c
d42d7012-6d6a-4b54-aa28-e8c2ebff9b1a	619711dc-e907-4b64-bf75-d64e657a2790	ffced565-d714-42ce-a4f1-995c9511441c	ASIGNADO	SIN_ASIGNAR	ASIGNADO	\N	\N	\N	\N	2026-07-21 14:30:50.192431+00	ffced565-d714-42ce-a4f1-995c9511441c
f311b53d-8a52-4fc4-85ba-ad647d9fc97c	619711dc-e907-4b64-bf75-d64e657a2790	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	ASIGNADO	EN_PROCESO	\N	\N	\N	\N	2026-07-27 14:54:39.926349+00	ffced565-d714-42ce-a4f1-995c9511441c
d0945044-893a-4d8a-ada3-a3de814fd2cf	fb23048f-b94a-41ba-9353-f1754f02fb61	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-07-31 01:33:17.458878+00	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea
556f0462-ea6d-490b-b0c3-55e2d5a6ea31	fb23048f-b94a-41ba-9353-f1754f02fb61	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-07-31 01:33:17.519536+00	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea
473d971d-373f-41aa-b942-d6287f26113e	fb23048f-b94a-41ba-9353-f1754f02fb61	21540103-61cc-4141-a3f1-11763957b648	ASIGNADO	SIN_ASIGNAR	ASIGNADO	\N	\N	\N	\N	2026-07-31 01:35:03.38099+00	21540103-61cc-4141-a3f1-11763957b648
769b6e13-0d1f-48d6-be16-3faf4cfab2c8	487f9484-9b26-42b8-8439-395d9625fe19	ffced565-d714-42ce-a4f1-995c9511441c	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-08-03 14:36:27.702701+00	ffced565-d714-42ce-a4f1-995c9511441c
9ede9dff-89fb-4473-87a8-d5e172ccc49a	487f9484-9b26-42b8-8439-395d9625fe19	ffced565-d714-42ce-a4f1-995c9511441c	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-08-03 14:36:27.789967+00	ffced565-d714-42ce-a4f1-995c9511441c
12b8f358-ef09-49eb-9df3-66ab361e304b	487f9484-9b26-42b8-8439-395d9625fe19	ffced565-d714-42ce-a4f1-995c9511441c	COMENTADO	\N	\N	soporte prueba	\N	\N	\N	2026-08-03 14:51:05.347758+00	ffced565-d714-42ce-a4f1-995c9511441c
695d70bf-8805-4134-9bf2-abef249a8ab4	487f9484-9b26-42b8-8439-395d9625fe19	ffced565-d714-42ce-a4f1-995c9511441c	EVIDENCIA_SUBIDA	\N	\N	\N	\N	\N	\N	2026-08-03 14:52:04.710743+00	ffced565-d714-42ce-a4f1-995c9511441c
0a9a5715-7bbb-4cef-ab50-74d1bec1de05	487f9484-9b26-42b8-8439-395d9625fe19	ffced565-d714-42ce-a4f1-995c9511441c	ASIGNADO	SIN_ASIGNAR	ASIGNADO	\N	\N	\N	\N	2026-08-03 14:53:35.226893+00	ffced565-d714-42ce-a4f1-995c9511441c
235ca2a7-dfa5-4ae8-9652-910f8e74c8f7	487f9484-9b26-42b8-8439-395d9625fe19	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	ASIGNADO	EN_PROCESO	\N	\N	\N	\N	2026-08-03 15:48:52.728616+00	ffced565-d714-42ce-a4f1-995c9511441c
cd31d796-cf11-4f7e-8094-4ce999f40aed	c4a88eb1-027c-407d-b024-e9ad07a17bd1	25541c55-aafe-4714-98d1-a177b057302e	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-08-03 16:15:59.601095+00	25541c55-aafe-4714-98d1-a177b057302e
3f5ac5e5-a5fa-4143-9554-7820632319eb	c4a88eb1-027c-407d-b024-e9ad07a17bd1	25541c55-aafe-4714-98d1-a177b057302e	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-08-03 16:15:59.634534+00	25541c55-aafe-4714-98d1-a177b057302e
073a68d3-ec6e-4f0d-9dc7-3032a9f69958	c4a88eb1-027c-407d-b024-e9ad07a17bd1	25541c55-aafe-4714-98d1-a177b057302e	EVIDENCIA_SUBIDA	\N	\N	\N	\N	\N	\N	2026-08-03 16:16:02.600053+00	25541c55-aafe-4714-98d1-a177b057302e
066de69e-d6fd-4971-9136-256372ae84df	c4a88eb1-027c-407d-b024-e9ad07a17bd1	ffced565-d714-42ce-a4f1-995c9511441c	COMENTADO	\N	\N	nuevo ticket creado1	\N	\N	\N	2026-08-03 16:17:59.10235+00	ffced565-d714-42ce-a4f1-995c9511441c
da77f3fb-70eb-45ff-a91c-ffa5f3bf8fb5	c4a88eb1-027c-407d-b024-e9ad07a17bd1	ffced565-d714-42ce-a4f1-995c9511441c	ASIGNADO	SIN_ASIGNAR	ASIGNADO	\N	\N	\N	\N	2026-08-03 16:19:14.131417+00	ffced565-d714-42ce-a4f1-995c9511441c
66c16213-2f74-4d0c-8bb7-6842e413056b	c4a88eb1-027c-407d-b024-e9ad07a17bd1	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	ASIGNADO	EN_PROCESO	\N	\N	\N	\N	2026-08-03 16:20:59.518625+00	ffced565-d714-42ce-a4f1-995c9511441c
f1be7535-782c-4ea8-87e5-a5b13ddd8455	c4a88eb1-027c-407d-b024-e9ad07a17bd1	ffced565-d714-42ce-a4f1-995c9511441c	COMENTADO	\N	\N	ticket nuevo 1	\N	\N	\N	2026-08-03 16:22:29.413556+00	ffced565-d714-42ce-a4f1-995c9511441c
e28af15d-c302-4d4b-a772-06ff3e1ef95b	c4a88eb1-027c-407d-b024-e9ad07a17bd1	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	EN_PROCESO	PENDIENTE_VALIDACION	\N	\N	\N	\N	2026-08-03 16:24:20.081496+00	ffced565-d714-42ce-a4f1-995c9511441c
3fe3aa1f-b225-4a70-b5bc-f4f848d42f7d	c4a88eb1-027c-407d-b024-e9ad07a17bd1	ffced565-d714-42ce-a4f1-995c9511441c	COMENTADO	\N	\N	aprobado	\N	\N	\N	2026-08-03 17:54:21.944682+00	ffced565-d714-42ce-a4f1-995c9511441c
df7c6110-fde8-492a-ae02-f3492ce7aa79	c4a88eb1-027c-407d-b024-e9ad07a17bd1	25541c55-aafe-4714-98d1-a177b057302e	COMENTADO	\N	\N	aprobado	\N	\N	\N	2026-08-03 17:54:54.8198+00	25541c55-aafe-4714-98d1-a177b057302e
0325763c-32ba-439e-ab24-ab9c00d186c0	c4a88eb1-027c-407d-b024-e9ad07a17bd1	25541c55-aafe-4714-98d1-a177b057302e	ESTADO_CAMBIADO	PENDIENTE_VALIDACION	REABIERTO	\N	d6ab1693-9f5a-4f6b-a9ca-4ebdc02828ea	hola	\N	2026-08-03 19:51:58.04904+00	25541c55-aafe-4714-98d1-a177b057302e
642d8f48-e05f-4c60-9ea0-b343d945db0e	c4a88eb1-027c-407d-b024-e9ad07a17bd1	ffced565-d714-42ce-a4f1-995c9511441c	COMENTADO	\N	\N	.	\N	\N	\N	2026-08-03 20:38:57.488009+00	ffced565-d714-42ce-a4f1-995c9511441c
e519b63c-f9d1-4147-a61a-69861c99357f	e3842e17-5cfc-4cbb-a504-010a3293a55a	25541c55-aafe-4714-98d1-a177b057302e	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-08-03 20:50:27.76734+00	25541c55-aafe-4714-98d1-a177b057302e
2c914b74-67b1-4d31-b33e-af7c0dec5521	e3842e17-5cfc-4cbb-a504-010a3293a55a	25541c55-aafe-4714-98d1-a177b057302e	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-08-03 20:50:27.8071+00	25541c55-aafe-4714-98d1-a177b057302e
586ff381-d54c-4454-9d08-672ba34dacdc	e3842e17-5cfc-4cbb-a504-010a3293a55a	25541c55-aafe-4714-98d1-a177b057302e	EVIDENCIA_SUBIDA	\N	\N	\N	\N	\N	\N	2026-08-03 20:50:30.881154+00	25541c55-aafe-4714-98d1-a177b057302e
f9a6c402-a80d-403b-977b-0961be8d824a	e3842e17-5cfc-4cbb-a504-010a3293a55a	25541c55-aafe-4714-98d1-a177b057302e	EVIDENCIA_SUBIDA	\N	\N	\N	\N	\N	\N	2026-08-03 20:50:31.526202+00	25541c55-aafe-4714-98d1-a177b057302e
9d1e45a4-f24b-4757-a50f-205f0b48e175	e3842e17-5cfc-4cbb-a504-010a3293a55a	25541c55-aafe-4714-98d1-a177b057302e	ESTADO_CAMBIADO	SIN_ASIGNAR	CANCELADO	\N	\N	\N	\N	2026-08-03 20:52:40.804636+00	25541c55-aafe-4714-98d1-a177b057302e
c05aaf37-11bd-477b-bd4c-4e8e8ab83511	26bb057a-9c13-436f-9d2b-1178aa1ad034	25541c55-aafe-4714-98d1-a177b057302e	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-08-03 21:00:47.50524+00	25541c55-aafe-4714-98d1-a177b057302e
dada60fb-63a0-4ef7-86db-548ff02205ce	26bb057a-9c13-436f-9d2b-1178aa1ad034	25541c55-aafe-4714-98d1-a177b057302e	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-08-03 21:00:47.53634+00	25541c55-aafe-4714-98d1-a177b057302e
4a7f09b9-fa61-464d-9423-38b821ae72d2	26bb057a-9c13-436f-9d2b-1178aa1ad034	25541c55-aafe-4714-98d1-a177b057302e	EVIDENCIA_SUBIDA	\N	\N	\N	\N	\N	\N	2026-08-03 21:00:49.275848+00	25541c55-aafe-4714-98d1-a177b057302e
557e4180-2a5a-40e0-9010-3aa2684d9252	26bb057a-9c13-436f-9d2b-1178aa1ad034	25541c55-aafe-4714-98d1-a177b057302e	COMENTADO	\N	\N	se va cancelar el ticket porque ya no se necesita soporte , fue solucionado internamente	\N	\N	\N	2026-08-03 21:04:37.580064+00	25541c55-aafe-4714-98d1-a177b057302e
a30b8b34-072a-473a-8c58-9fd39dc9c84a	26bb057a-9c13-436f-9d2b-1178aa1ad034	25541c55-aafe-4714-98d1-a177b057302e	ESTADO_CAMBIADO	SIN_ASIGNAR	CANCELADO	\N	\N	\N	\N	2026-08-03 21:08:17.178008+00	25541c55-aafe-4714-98d1-a177b057302e
8907060d-32af-4b4a-95e7-8effa86bdc83	c4a88eb1-027c-407d-b024-e9ad07a17bd1	ffced565-d714-42ce-a4f1-995c9511441c	DATOS_ACTUALIZADOS	\N	\N	\N	\N	\N	{"tipoAnterior": "641ca311-3280-4faa-ac49-285822a16f02", "tituloAnterior": "nuevo ticket de prueba"}	2026-08-03 21:27:34.414892+00	ffced565-d714-42ce-a4f1-995c9511441c
87d18ffd-7e84-41ec-a4a6-2fd5f1fd98f4	487f9484-9b26-42b8-8439-395d9625fe19	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	EN_PROCESO	CANCELADO	\N	\N	\N	\N	2026-08-04 01:17:18.860645+00	ffced565-d714-42ce-a4f1-995c9511441c
d8e15a42-dc93-4ccd-833c-3b142e38dd4b	8ba5f1be-4ce8-434d-be41-3f6edcc59e4f	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	SIN_ASIGNAR	CANCELADO	\N	\N	\N	\N	2026-08-04 01:18:04.822156+00	ffced565-d714-42ce-a4f1-995c9511441c
23c9b600-68d5-4231-81ba-7823784e30db	d9cb455e-d34a-4ef1-bf19-14068044a85a	ffced565-d714-42ce-a4f1-995c9511441c	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-08-04 16:04:27.080233+00	ffced565-d714-42ce-a4f1-995c9511441c
b303dbbe-d405-411d-b2c7-0af3f2ad1eea	d9cb455e-d34a-4ef1-bf19-14068044a85a	ffced565-d714-42ce-a4f1-995c9511441c	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-08-04 16:04:27.141942+00	ffced565-d714-42ce-a4f1-995c9511441c
64985114-bddd-4357-9da6-359fef4573e7	d9cb455e-d34a-4ef1-bf19-14068044a85a	ffced565-d714-42ce-a4f1-995c9511441c	EVIDENCIA_SUBIDA	\N	\N	\N	\N	\N	\N	2026-08-04 16:04:30.399232+00	ffced565-d714-42ce-a4f1-995c9511441c
36d51182-e322-4284-aa11-6d32a576c99f	d9cb455e-d34a-4ef1-bf19-14068044a85a	ffced565-d714-42ce-a4f1-995c9511441c	COMENTADO	\N	\N	creacion de nuevo ticket	\N	\N	\N	2026-08-04 16:09:05.106371+00	ffced565-d714-42ce-a4f1-995c9511441c
e8b09fcb-cc6e-463d-bdf6-403b8dd73487	d9cb455e-d34a-4ef1-bf19-14068044a85a	ffced565-d714-42ce-a4f1-995c9511441c	ASIGNADO	SIN_ASIGNAR	ASIGNADO	\N	\N	\N	\N	2026-08-04 16:09:19.501785+00	ffced565-d714-42ce-a4f1-995c9511441c
88d57b5d-aab2-4563-9a5b-8e0e82905d8a	d9cb455e-d34a-4ef1-bf19-14068044a85a	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	ASIGNADO	EN_PROCESO	\N	\N	\N	\N	2026-08-04 16:10:07.391276+00	ffced565-d714-42ce-a4f1-995c9511441c
f8e71363-ef1d-4ae3-a829-1b4f24cf71f0	d9cb455e-d34a-4ef1-bf19-14068044a85a	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	EN_PROCESO	EN_ESPERA	\N	\N	\N	\N	2026-08-04 16:16:34.20267+00	ffced565-d714-42ce-a4f1-995c9511441c
87728519-233b-4825-8734-b34757680aae	d9cb455e-d34a-4ef1-bf19-14068044a85a	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	EN_ESPERA	EN_PROCESO	\N	\N	\N	\N	2026-08-04 16:16:39.697102+00	ffced565-d714-42ce-a4f1-995c9511441c
a60e9372-2198-49c1-ba4c-0324f81655cc	d9cb455e-d34a-4ef1-bf19-14068044a85a	ffced565-d714-42ce-a4f1-995c9511441c	COMENTADO	\N	\N	prueba	\N	\N	\N	2026-08-04 16:16:41.799833+00	ffced565-d714-42ce-a4f1-995c9511441c
85c86ecc-ce53-436e-8537-b57d0e9ed295	d9cb455e-d34a-4ef1-bf19-14068044a85a	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	EN_PROCESO	PENDIENTE_VALIDACION	\N	\N	\N	\N	2026-08-04 16:16:47.600484+00	ffced565-d714-42ce-a4f1-995c9511441c
b695a63d-a2ce-4e39-859b-0c798c7cffb8	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-08-04 17:07:08.522454+00	a72dcddd-405e-4d93-a819-d6f16bfc5f1c
dc91a0ba-f561-49dd-bb9c-4f958d05f868	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-08-04 17:07:08.552193+00	a72dcddd-405e-4d93-a819-d6f16bfc5f1c
a11f58f5-0de2-4633-b4d4-ae2e33ca582e	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	EVIDENCIA_SUBIDA	\N	\N	\N	\N	\N	\N	2026-08-04 17:07:11.416489+00	a72dcddd-405e-4d93-a819-d6f16bfc5f1c
b046d7f4-51a2-49f3-bc10-c75e2b82eb95	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	ffced565-d714-42ce-a4f1-995c9511441c	ASIGNADO	SIN_ASIGNAR	ASIGNADO	\N	\N	\N	\N	2026-08-04 17:07:46.235011+00	ffced565-d714-42ce-a4f1-995c9511441c
f8421b91-f7b5-4187-8461-9578a0eea598	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	COMENTADO	\N	\N	ticket de prueba	\N	\N	\N	2026-08-04 17:15:04.730856+00	a72dcddd-405e-4d93-a819-d6f16bfc5f1c
ffa94e26-ce2b-41cc-b5ca-8ff018f34c0e	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	ASIGNADO	EN_PROCESO	\N	\N	\N	\N	2026-08-04 17:16:48.313957+00	ffced565-d714-42ce-a4f1-995c9511441c
7ab0be34-80f8-4d23-91ca-3a3173e1f6aa	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	EN_PROCESO	EN_ESPERA	\N	\N	\N	\N	2026-08-04 17:16:55.839318+00	ffced565-d714-42ce-a4f1-995c9511441c
f172ff3b-d800-448c-bf1a-df850ca9e525	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	EN_ESPERA	EN_PROCESO	\N	\N	\N	\N	2026-08-04 17:17:02.721747+00	ffced565-d714-42ce-a4f1-995c9511441c
8bbf289a-9071-4dbd-9f26-3a99dfe6dc9b	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	EN_PROCESO	PENDIENTE_VALIDACION	\N	\N	\N	\N	2026-08-04 17:17:16.806871+00	ffced565-d714-42ce-a4f1-995c9511441c
c2a5522e-a2a9-4724-b8e2-50d187d8975b	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	COMENTADO	\N	\N	.	\N	\N	\N	2026-08-04 17:20:09.311664+00	a72dcddd-405e-4d93-a819-d6f16bfc5f1c
f64c9d5b-c367-463e-8124-f23b5123636c	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	ffced565-d714-42ce-a4f1-995c9511441c	COMENTADO	\N	\N	.	\N	\N	\N	2026-08-04 17:31:53.625545+00	ffced565-d714-42ce-a4f1-995c9511441c
3cea24b5-6011-4089-94a1-7c5b05d334ae	bf8b3d6d-1ba6-4e5e-b8de-44161fbd6400	ffced565-d714-42ce-a4f1-995c9511441c	COMENTADO	\N	\N	no deja cerrar el ticket	\N	\N	\N	2026-08-04 19:25:42.198303+00	ffced565-d714-42ce-a4f1-995c9511441c
36c46fb5-55ef-45bf-afb3-e0f458633ca1	c4a88eb1-027c-407d-b024-e9ad07a17bd1	21540103-61cc-4141-a3f1-11763957b648	ASIGNADO	REABIERTO	ASIGNADO	\N	\N	\N	\N	2026-08-06 19:50:37.031539+00	21540103-61cc-4141-a3f1-11763957b648
750f1b69-0e97-4d40-a430-1ed6e46a22ad	62b633a8-4a8b-4bca-9d00-8f098c6b2452	21540103-61cc-4141-a3f1-11763957b648	ASIGNADO	SIN_ASIGNAR	ASIGNADO	\N	\N	\N	\N	2026-08-06 19:51:26.552785+00	21540103-61cc-4141-a3f1-11763957b648
e54cc85c-f575-4e03-9458-f3c1a169a279	f856d580-0907-4f8f-ae86-7007cf8d527b	ffced565-d714-42ce-a4f1-995c9511441c	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-08-12 13:42:10.519643+00	ffced565-d714-42ce-a4f1-995c9511441c
4a5485b3-daa1-42dc-83db-8fe776688295	f856d580-0907-4f8f-ae86-7007cf8d527b	ffced565-d714-42ce-a4f1-995c9511441c	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-08-12 13:42:10.601606+00	ffced565-d714-42ce-a4f1-995c9511441c
042bcf03-abc5-48cb-a659-754f25e6e4d4	f856d580-0907-4f8f-ae86-7007cf8d527b	ffced565-d714-42ce-a4f1-995c9511441c	EVIDENCIA_SUBIDA	\N	\N	\N	\N	\N	\N	2026-08-12 13:42:14.411054+00	ffced565-d714-42ce-a4f1-995c9511441c
47d33ee7-e1bc-4e8f-84d5-0cbbbeab5ac1	f856d580-0907-4f8f-ae86-7007cf8d527b	ffced565-d714-42ce-a4f1-995c9511441c	EVIDENCIA_SUBIDA	\N	\N	\N	\N	\N	\N	2026-08-12 13:42:14.509691+00	ffced565-d714-42ce-a4f1-995c9511441c
15295bfb-90b4-493c-a2e1-ae0d9ac4b7d0	f856d580-0907-4f8f-ae86-7007cf8d527b	ffced565-d714-42ce-a4f1-995c9511441c	EVIDENCIA_SUBIDA	\N	\N	\N	\N	\N	\N	2026-08-12 13:42:14.517835+00	ffced565-d714-42ce-a4f1-995c9511441c
105c75dd-abd0-4c0d-aa09-8ca393313c20	f856d580-0907-4f8f-ae86-7007cf8d527b	ffced565-d714-42ce-a4f1-995c9511441c	COMENTADO	\N	\N	hola	\N	\N	\N	2026-08-12 13:43:54.339955+00	ffced565-d714-42ce-a4f1-995c9511441c
d2165202-f254-4a0d-9816-559aba74894b	f856d580-0907-4f8f-ae86-7007cf8d527b	ffced565-d714-42ce-a4f1-995c9511441c	ASIGNADO	SIN_ASIGNAR	ASIGNADO	\N	\N	\N	\N	2026-08-12 13:44:03.709067+00	ffced565-d714-42ce-a4f1-995c9511441c
ca5fa3d5-0d34-42a6-a24a-541e878ca9a2	f856d580-0907-4f8f-ae86-7007cf8d527b	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	ASIGNADO	EN_PROCESO	\N	\N	\N	\N	2026-08-12 14:03:38.663004+00	ffced565-d714-42ce-a4f1-995c9511441c
4ef445a7-3add-4d08-addc-166f6b4198c0	f856d580-0907-4f8f-ae86-7007cf8d527b	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	EN_PROCESO	PENDIENTE_VALIDACION	\N	\N	\N	\N	2026-08-12 14:06:14.420047+00	ffced565-d714-42ce-a4f1-995c9511441c
070649a0-f323-4d07-9e14-f7b076e7a2c3	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-08-12 14:26:55.033744+00	ffced565-d714-42ce-a4f1-995c9511441c
3155c8b9-813c-4403-b5a1-a312116244f1	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-08-12 14:26:55.06912+00	ffced565-d714-42ce-a4f1-995c9511441c
cd9754ac-0934-4a14-9fe1-cdc92d5d521d	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	EVIDENCIA_SUBIDA	\N	\N	\N	\N	\N	\N	2026-08-12 14:26:57.222151+00	ffced565-d714-42ce-a4f1-995c9511441c
f010d583-3d04-4075-9985-cf279c4f153c	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	EVIDENCIA_SUBIDA	\N	\N	\N	\N	\N	\N	2026-08-12 14:26:57.225785+00	ffced565-d714-42ce-a4f1-995c9511441c
2056eccd-88f1-454b-8f03-74166cf7f2e6	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	ASIGNADO	SIN_ASIGNAR	ASIGNADO	\N	\N	\N	\N	2026-08-12 14:27:30.826469+00	ffced565-d714-42ce-a4f1-995c9511441c
f9b00f14-9f81-4763-bcec-880a0ae57682	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	COMENTADO	\N	\N	hola	\N	\N	\N	2026-08-12 14:29:16.027287+00	ffced565-d714-42ce-a4f1-995c9511441c
f20deb68-04dc-42a9-88d6-5e49dd643839	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	COMENTADO	\N	\N	hola	\N	\N	\N	2026-08-12 14:30:56.522241+00	ffced565-d714-42ce-a4f1-995c9511441c
b8ac9ca7-add4-463d-b2a9-ee00fd8d6a1d	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	ASIGNADO	EN_PROCESO	\N	\N	\N	\N	2026-08-17 14:54:38.318588+00	ffced565-d714-42ce-a4f1-995c9511441c
b14922a9-931e-49ad-86f1-45bb85806c91	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	EN_PROCESO	EN_ESPERA	\N	\N	\N	\N	2026-08-17 14:55:26.459652+00	ffced565-d714-42ce-a4f1-995c9511441c
cb0490fd-fded-44cd-8995-065338480e62	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	EN_ESPERA	EN_PROCESO	\N	\N	\N	\N	2026-08-17 14:55:35.346667+00	ffced565-d714-42ce-a4f1-995c9511441c
a83dfe9c-03a3-44a5-921f-1518f62bfdd0	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	EN_PROCESO	PENDIENTE_VALIDACION	\N	\N	\N	\N	2026-08-17 14:55:43.354402+00	ffced565-d714-42ce-a4f1-995c9511441c
5fffbf2b-7f22-4d4e-9470-f33da5fc64d0	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	PENDIENTE_VALIDACION	REABIERTO	\N	d6ab1693-9f5a-4f6b-a9ca-4ebdc02828ea	e	\N	2026-08-17 14:58:30.367214+00	ffced565-d714-42ce-a4f1-995c9511441c
a5fb0181-d423-4f4d-ad66-fa121ab3b0eb	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	COMENTADO	\N	\N	f	\N	\N	\N	2026-08-17 14:58:45.562778+00	ffced565-d714-42ce-a4f1-995c9511441c
dbd225f1-ca77-40c8-91c9-09de98c83c7d	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	ffced565-d714-42ce-a4f1-995c9511441c	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-08-18 17:42:55.604536+00	ffced565-d714-42ce-a4f1-995c9511441c
6f37d0b3-55cb-466a-a015-438ac0bc797f	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	ffced565-d714-42ce-a4f1-995c9511441c	CREADO	\N	SIN_ASIGNAR	\N	\N	\N	\N	2026-08-18 17:42:55.698283+00	ffced565-d714-42ce-a4f1-995c9511441c
63fb8173-ec2c-46ff-b2ca-cf6e4b0aa0e8	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	ffced565-d714-42ce-a4f1-995c9511441c	EVIDENCIA_SUBIDA	\N	\N	\N	\N	\N	\N	2026-08-18 17:42:58.470871+00	ffced565-d714-42ce-a4f1-995c9511441c
785805dc-f848-4152-a46d-fea423f15fb3	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	ffced565-d714-42ce-a4f1-995c9511441c	COMENTADO	\N	\N	Hola	\N	\N	\N	2026-08-18 17:43:52.439587+00	ffced565-d714-42ce-a4f1-995c9511441c
8354dcb1-0a4c-49e0-a0aa-b1465dee599c	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	ffced565-d714-42ce-a4f1-995c9511441c	ASIGNADO	SIN_ASIGNAR	ASIGNADO	\N	\N	\N	\N	2026-08-18 17:51:20.541071+00	ffced565-d714-42ce-a4f1-995c9511441c
4729aa00-7a89-4173-9511-71a563b60e63	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	ASIGNADO	EN_PROCESO	\N	\N	\N	\N	2026-08-18 19:37:24.437192+00	ffced565-d714-42ce-a4f1-995c9511441c
c2aab10a-0c02-4d58-a6b5-8c9415f0aebc	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	EN_PROCESO	PENDIENTE_VALIDACION	\N	\N	\N	\N	2026-08-18 19:38:43.908431+00	ffced565-d714-42ce-a4f1-995c9511441c
c2231918-391e-42f1-897b-e54f5cb16398	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	ffced565-d714-42ce-a4f1-995c9511441c	COMENTADO	\N	\N	hola .	\N	\N	\N	2026-08-18 19:45:40.624006+00	ffced565-d714-42ce-a4f1-995c9511441c
af5db7c1-5704-4629-aa20-b79722e809f6	f2de07d3-d7c2-4ba3-9691-5c12bc13b23a	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	PENDIENTE_VALIDACION	CERRADO	\N	\N	\N	\N	2026-08-18 19:45:58.934893+00	ffced565-d714-42ce-a4f1-995c9511441c
c66f462d-3a73-4fef-9e59-ca49fff32761	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	ASIGNADO	REABIERTO	ASIGNADO	\N	\N	\N	\N	2026-08-18 19:51:16.810766+00	ffced565-d714-42ce-a4f1-995c9511441c
9216ff0f-69ba-4360-ab44-75b585245317	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	ASIGNADO	EN_PROCESO	\N	\N	\N	\N	2026-08-18 19:51:26.10608+00	ffced565-d714-42ce-a4f1-995c9511441c
7b2674bc-033d-43ad-a0f6-55dc111a6262	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	EN_PROCESO	EN_ESPERA	\N	\N	\N	\N	2026-08-18 19:51:37.127182+00	ffced565-d714-42ce-a4f1-995c9511441c
3da3f07f-e51f-42e4-8039-ebfeaf7fc914	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	EN_ESPERA	EN_PROCESO	\N	\N	\N	\N	2026-08-18 19:52:44.224539+00	ffced565-d714-42ce-a4f1-995c9511441c
313cb20f-46b2-4b06-aabf-b721aa80e183	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	EN_PROCESO	PENDIENTE_VALIDACION	\N	\N	\N	\N	2026-08-18 19:54:31.480147+00	ffced565-d714-42ce-a4f1-995c9511441c
8f176b46-5e51-4ec0-93a0-12ad05269e9d	9b008df6-460a-4b34-b6cb-1d1ed878f6ab	ffced565-d714-42ce-a4f1-995c9511441c	ESTADO_CAMBIADO	PENDIENTE_VALIDACION	REABIERTO	\N	604261f8-ae9b-4c9f-b6d7-bccb97cf6732	hola	\N	2026-08-18 19:55:36.012782+00	ffced565-d714-42ce-a4f1-995c9511441c
\.


ALTER TABLE public.ticket_historial ENABLE TRIGGER ALL;

--
-- Data for Name: usuario_sucursales; Type: TABLE DATA; Schema: public; Owner: -
--

ALTER TABLE public.usuario_sucursales DISABLE TRIGGER ALL;

COPY public.usuario_sucursales (id, usuario_id, sucursal_id, es_principal, activo, created_at, created_by, updated_at, updated_by) FROM stdin;
e2ab0c21-0649-4912-beb7-fbdb1b616176	02419c75-3006-4f51-8019-a435201f52ba	1330cb6d-2785-488e-8710-adcb6d2fceb7	t	t	2026-07-22 02:32:33.131737+00	\N	\N	\N
76cdd746-3a18-4e91-82e8-e02648c33c50	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	07c96055-9c8d-4aa0-8fe6-84cb70c532d2	f	t	2026-07-27 20:08:59.864119+00	21540103-61cc-4141-a3f1-11763957b648	2026-07-27 20:08:59.864119+00	\N
2c51d974-36b1-4033-a22b-96c644a82fb3	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	a48d3ab2-9057-45a4-980b-ce28ef27f028	f	t	2026-07-27 20:08:59.86412+00	21540103-61cc-4141-a3f1-11763957b648	2026-07-27 20:08:59.86412+00	\N
8d94b0b8-6512-4379-a53b-0db60cfb2f78	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	a48bcd1f-39ca-4071-9c54-7a5f412c3d1a	f	t	2026-07-27 20:08:59.864122+00	21540103-61cc-4141-a3f1-11763957b648	2026-07-27 20:08:59.864122+00	\N
a707467a-b9ca-4def-81c5-efc32ca00d53	7eaf59d2-415a-4297-ae8a-032d1ab8d2ea	1330cb6d-2785-488e-8710-adcb6d2fceb7	t	t	2026-07-27 20:08:59.864112+00	21540103-61cc-4141-a3f1-11763957b648	2026-07-27 20:08:59.864112+00	\N
3ba61941-2dcb-4ce6-9323-a61db7145983	25541c55-aafe-4714-98d1-a177b057302e	1330cb6d-2785-488e-8710-adcb6d2fceb7	f	t	2026-08-03 15:57:41.032825+00	ffced565-d714-42ce-a4f1-995c9511441c	2026-08-03 15:57:41.032825+00	\N
2d926be1-6857-45a3-8903-87ea664f4333	25541c55-aafe-4714-98d1-a177b057302e	d7459f2e-2bdc-4d31-8f7d-ca37f3732706	t	t	2026-08-03 15:57:41.111364+00	ffced565-d714-42ce-a4f1-995c9511441c	2026-08-03 15:57:41.111364+00	\N
8ad3d559-aa59-4e85-bce4-9c05863da933	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	a48d3ab2-9057-45a4-980b-ce28ef27f028	f	t	2026-08-18 17:50:59.527925+00	ffced565-d714-42ce-a4f1-995c9511441c	2026-08-18 17:50:59.527925+00	\N
6e7dca9e-d11b-4376-a8c5-d5180e2db64a	a72dcddd-405e-4d93-a819-d6f16bfc5f1c	d7459f2e-2bdc-4d31-8f7d-ca37f3732706	t	t	2026-08-18 17:50:59.52765+00	ffced565-d714-42ce-a4f1-995c9511441c	2026-08-18 17:50:59.52765+00	\N
\.


ALTER TABLE public.usuario_sucursales ENABLE TRIGGER ALL;

--
-- Name: ticket_codigo_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.ticket_codigo_seq', 45, true);


--
-- PostgreSQL database dump complete
--

\unrestrict H0TrnMfxzvtevbNfFOy40q5XpOAY8EhhLImcGyDvaw1WiOEs9XR0roGyrS5SQIk

