# Suportsaaron — guía de conexión y publicación

## Requisitos
- Visual Studio 2022 actualizado con la carga de trabajo **ASP.NET y desarrollo web**.
- SDK de .NET 10.
- SQL Server Management Studio (SSMS).
- Cuenta de MonsterASP.NET con un sitio web y una base MSSQL creados en el mismo plan.

## 1. Abrir y probar localmente
1. Descomprime `SUPORTSAARON_PROYECTO.zip`.
2. Abre `BlogCoreSolution.slnx` con Visual Studio.
3. Comprueba que el proyecto de inicio sea `BlogCore`.
4. En `BlogCore/appsettings.json`, deja la conexión local de SQL Server Express/LocalDB si tienes LocalDB instalado:
   `Server=(localdb)\\MSSQLLocalDB;Database=Suportsaaron;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True`
5. Ejecuta con **Ctrl+F5**. Al iniciar, la aplicación aplica las migraciones y crea los roles, cuentas, 4 categorías, 24 artículos y 4 sliders de demostración si las tablas están vacías.
6. Entra a `/Identity/Account/Login`. Las cuentas de prueba están en la tabla del informe PDF.
7. Como administrador, abre `/Admin/Categorias`, `/Admin/Articulos` y `/Admin/Sliders`. Los registros que guardes se consultan desde SQL Server y aparecen en la página pública al volver al inicio.

> Las imágenes y descripciones iniciales son de demostración. Sustitúyelas por fotografías optimizadas y descripciones originales del catálogo real antes de entregar.

## 2. Crear la base de datos en MonsterASP.NET
1. Inicia sesión en el panel de MonsterASP.NET.
2. Abre **Databases → Add database**.
3. Selecciona **SQL Server 2025** y el mismo tipo de plan (Free o Premium) que utilizarás para el sitio web. El sitio y la base de datos deben estar en el mismo plan para usar la conexión local.
4. Abre la base creada y entra a **Users → Connection string**. Copia la cadena **Local access** para la aplicación alojada.
5. Conserva la cadena **Remote access** para conectarte desde tu computadora con SSMS. No compartas contraseñas ni las publiques en GitHub.

Documentación oficial:
- Crear base: https://help.monsterasp.net/databases/mssql/create-database
- Cadenas de conexión: https://help.monsterasp.net/databases/mssql/connection-string

## 3. Conectar SSMS a la base remota de MonsterASP
1. En el panel de MonsterASP abre **Databases → tu base → Remote access** y habilita el acceso remoto. Si el panel permite limitar IP, autoriza solamente tu IP pública actual.
2. Abre SSMS → **Connect → Database Engine**.
3. En **Server name**, pega el servidor que aparece en la cadena remota (`Server=...`).
4. En **Authentication**, elige **SQL Server Authentication**.
5. En **Login** y **Password**, utiliza `User Id` y `Password` de la cadena remota.
6. En **Options → Connection Properties**, puedes indicar el nombre de la base de datos.
7. Pulsa **Connect**. Expande **Databases → tu base → Tables**. Después de ejecutar la aplicación y aplicar migraciones, verás tablas como `AspNetUsers`, `AspNetRoles`, `Categorias`, `Articulos` y `Sliders`.

No uses `localhost` ni la cadena Local access en SSMS desde tu computadora. Para SSMS desde fuera del hosting se necesita la cadena Remote access y remote access habilitado.

Documentación oficial:
- Conectar SSMS: https://help.monsterasp.net/databases/mssql/ssms
- Permitir acceso remoto: https://help.monsterasp.net/databases/mssql/remote-access

## 4. Configurar la aplicación para la base alojada
La aplicación lee `ConnectionStrings:ConexionSQL`. Para publicar, configura esa clave con la cadena **Local access** de MonsterASP. No uses la cadena Remote access dentro de la aplicación que ya está alojada.

Opción recomendada: en Visual Studio, al publicar mediante el perfil de MonsterASP, abre **Show all settings → Database** y coloca la cadena Local access en los campos de cadena de conexión y migraciones de Entity Framework, si aparecen.

También puedes establecer la variable de entorno:
`ConnectionStrings__ConexionSQL`
con el valor Local access en la configuración del sitio, si tu panel ofrece variables de entorno.

No escribas contraseñas reales en el archivo que subirás a GitHub.

## 5. Publicar desde Visual Studio con Web Deploy
1. En MonsterASP abre **Websites → tu sitio → Deploy**.
2. En la tarjeta **Visual Studio / WebDeploy**, activa WebDeploy y descarga el perfil `.publishSettings`.
3. En Visual Studio, clic derecho al proyecto **BlogCore** → **Publish**.
4. Selecciona **Import profile** e importa el `.publishSettings`.
5. Abre **Show all settings**. Configura la cadena Local access de la base alojada en la sección Database / Entity Framework Migrations cuando esté disponible.
6. Elige `Release` y pulsa **Publish**.
7. Cuando termine, abre el dominio desde Overview → Show.
8. Inicia sesión con el usuario administrador, verifica el menú Administración y prueba guardar una categoría, un artículo y un slider.

Guía oficial:
https://help.monsterasp.net/websites/frameworks/aspnet-core-with-mssql
https://help.monsterasp.net/getting-started/first-steps/publish-first-application

## 6. Verificación final
- La página principal carga y muestra el carrusel.
- Hay 4 categorías.
- Hay al menos 24 artículos (6 por categoría).
- Hay 4 sliders activos.
- Un registro nuevo guardado desde Admin aparece en el listado y en la página pública.
- El rol Administrador accede a `/Admin`; los otros roles no deben tener acceso administrativo.
- Login, cierre de sesión, búsqueda, detalle y paginación funcionan.
- La web utiliza HTTPS.

## Importante sobre las credenciales de prueba
Las cuentas se crean automáticamente al iniciar por primera vez la aplicación sobre una base de datos migrada. Se conservan los correos y contraseñas del documento de requisitos. Son credenciales de evaluación: no las reutilices en producción ni publiques el repositorio con contraseñas reales. Si ya existen usuarios con esos correos, el inicializador no cambia sus contraseñas.
