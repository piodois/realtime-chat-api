# 🚀 Realtime Chat API

Un sistema de chat en tiempo real construido con .NET 8, SignalR, JWT Authentication y PostgreSQL.

## 📋 Características

- ✅ **Autenticación JWT** - Sistema seguro de autenticación
- ✅ **Chat en tiempo real** - Mensajería instantánea con SignalR
- ✅ **Salas de chat** - Públicas y privadas
- ✅ **Historial de mensajes** - Persistencia en base de datos
- ✅ **Estados de usuario** - Online/Offline en tiempo real
- ✅ **Clean Architecture** - Separación de responsabilidades
- ✅ **API RESTful** - Endpoints bien documentados
- ✅ **Swagger UI** - Documentación interactiva

## 🛠️ Stack Tecnológico

### Backend
- **.NET 8** - Framework principal
- **SignalR** - Comunicación en tiempo real
- **JWT Authentication** - Autenticación segura
- **Entity Framework Core** - ORM para base de datos
- **AutoMapper** - Mapeo de objetos
- **PostgreSQL** - Base de datos principal

### Herramientas
- **Swagger/OpenAPI** - Documentación de API
- **Docker** - Containerización (opcional)

## 🏗️ Arquitectura

```
src/
├── RealtimeChat.API/          # Capa de presentación (Controllers, Hubs)
├── RealtimeChat.Application/   # Lógica de aplicación (Services)
├── RealtimeChat.Infrastructure/ # Acceso a datos (DbContext, Repositories)
├── RealtimeChat.Domain/        # Entidades del dominio
└── RealtimeChat.Shared/        # DTOs y modelos compartidos
```

## 🚀 Instalación y Configuración

### Prerrequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 13+](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/downloads)

### Configuración de Base de Datos

1. **Instalar PostgreSQL**
2. **Crear base de datos**:
```sql
CREATE DATABASE RealtimeChatDb;
CREATE USER chatuser WITH PASSWORD 'yourpassword';
GRANT ALL PRIVILEGES ON DATABASE RealtimeChatDb TO chatuser;
```

### Configuración del Proyecto

1. **Clonar el repositorio**:
```bash
git clone https://github.com/piodois/realtime-chat-api.git
cd realtime-chat-api
```

2. **Restaurar dependencias**:
```bash
dotnet restore
```

3. **Configurar appsettings**:
   - Copia `appsettings.json` a `appsettings.Development.json`
   - Actualiza la cadena de conexión con tus credenciales de PostgreSQL

4. **Ejecutar migraciones** (si es necesario):
```bash
dotnet ef migrations add InitialCreate --project src/RealtimeChat.Infrastructure --startup-project src/RealtimeChat.API
dotnet ef database update --project src/RealtimeChat.Infrastructure --startup-project src/RealtimeChat.API
```

5. **Ejecutar la aplicación**:
```bash
dotnet run --project src/RealtimeChat.API
```

## 📚 Uso de la API

### URLs Principales

- **API Base**: `http://localhost:5000/api`
- **Swagger UI**: `http://localhost:5000/swagger`
- **SignalR Hub**: `ws://localhost:5000/chatHub`

### Endpoints Principales

#### Autenticación
```http
POST /api/auth/register
POST /api/auth/login
```

#### Chat
```http
GET    /api/chat/rooms           # Obtener salas del usuario
POST   /api/chat/rooms           # Crear nueva sala
POST   /api/chat/rooms/{id}/join # Unirse a una sala
GET    /api/chat/rooms/{id}/messages # Obtener mensajes
```

#### Health Check
```http
GET /api/health                  # Estado de la API
```

### Ejemplo de Registro
```json
POST /api/auth/register
{
  "username": "usuario123",
  "email": "usuario@ejemplo.com",
  "password": "password123"
}
```

### Ejemplo de Inicio de Sesión
```json
POST /api/auth/login
{
  "email": "usuario@ejemplo.com",
  "password": "password123"
}
```

## 🔌 Cliente SignalR

### Conectar al Hub
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub", {
        accessTokenFactory: () => localStorage.getItem("token")
    })
    .build();

// Conectar
await connection.start();
```

### Escuchar Mensajes
```javascript
connection.on("ReceiveMessage", (message) => {
    console.log("Nuevo mensaje:", message);
    // Actualizar UI
});
```

### Enviar Mensajes
```javascript
await connection.invoke("SendMessage", {
    content: "¡Hola mundo!",
    chatRoomId: "room-id-aqui"
});
```

### Unirse a Sala
```javascript
await connection.invoke("JoinRoom", "room-id-aqui");
```

## 🐳 Docker (Opcional)

```bash
# Ejecutar con Docker Compose
docker-compose up -d

# Solo la base de datos
docker-compose up postgres -d
```

## 🧪 Testing

```bash
# Ejecutar tests unitarios
dotnet test

# Ejecutar con cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 📈 Desarrollo

### Agregar Migración
```bash
dotnet ef migrations add NombreMigracion --project src/RealtimeChat.Infrastructure --startup-project src/RealtimeChat.API
```

### Aplicar Migración
```bash
dotnet ef database update --project src/RealtimeChat.Infrastructure --startup-project src/RealtimeChat.API
```

### Ejecutar en Desarrollo
```bash
dotnet watch run --project src/RealtimeChat.API
```


## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para detalles.

## 👨‍💻 Autor

**Tu Nombre**
- GitHub: [@Piodois](https://github.com/Piodois)
- LinkedIn: [Pio Cerda](https://www.linkedin.com/in/piocerda/)

## 🙏 Agradecimientos

- [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/) - Documentación de ASP.NET Core
- [SignalR Documentation](https://docs.microsoft.com/en-us/aspnet/core/signalr/) - Documentación de SignalR
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) - Documentación de EF Core

---

⭐ ¡No olvides dar una estrella si te gustó el proyecto!
