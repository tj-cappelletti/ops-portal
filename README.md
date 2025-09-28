# ops-portal

A lightweight, self-hosted operations portal for tracking infrastructure and applications across hybrid environments. Whether you're managing a homelab or a small enterprise infrastructure, ops-portal provides a unified view of your resources, deployments, and service health.

## 🎯 Key Features

- 🏠 **Hybrid Infrastructure** - Track resources across cloud providers and on-premises
- 🔍 **Service Discovery** - Automatic and manual service registration  
- 📊 **Health Monitoring** - Simple, effective monitoring without the complexity
- 🗺️ **Deployment Mapping** - Know what's deployed where at a glance
- 🔒 **Enterprise Security** - SSO support, private deployments, VPN-friendly
- 🎨 **Solution Stacks** - Organize related applications, repositories, and infrastructure together

## 🚀 Quick Start

### Local Development

```bash
# Clone the repository
git clone https://github.com/tj-cappelletti/ops-portal.git
cd ops-portal

# Copy environment template
cp .env.template .env

# Start with Docker Compose
docker-compose up -d

# Access the portal
open http://localhost:4200
```

### Production Deployment

For production deployments, we recommend using a private configuration repository. See our [Deployment Guide](docs/deployment-guide.md) for detailed instructions.

## 📚 Documentation

- [Architecture Overview](docs/architecture.md)
- [Deployment Guide](docs/deployment-guide.md)
- [Configuration Reference](docs/configuration.md)
- [API Documentation](docs/api.md)
- [Contributing Guide](CONTRIBUTING.md)

## 🏗️ Project Structure

```
ops-portal/
├── apps/
│   ├── api/                    # .NET Web API
│   ├── web/                    # Angular application
│   └── homelab-agent/          # Home lab discovery agent
├── infrastructure/
│   ├── bicep/                  # Azure deployment modules
│   ├── terraform/              # Home lab deployment modules
│   └── docker/                 # Docker configurations
├── packages/
│   └── shared/                 # Shared models and utilities
└── docs/                       # Documentation
```

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details on how to get started.

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔒 Security

For production use, please review our [Security Guide](docs/security.md) and never commit sensitive information to this repository.

## 🌟 Solution Stack Concept

A **Solution Stack** represents a complete solution to a business or technical need, encompassing all the code, infrastructure, and services required to deliver that solution. Each stack can include multiple repositories, deployment environments, and infrastructure components.

## 🚧 Project Status

This project is in active development. See our [Roadmap](https://github.com/tj-cappelletti/ops-portal/issues?q=is%3Aopen+is%3Aissue+label%3Aroadmap) for planned features.
