#!/bin/bash

# Expense Tracker Docker Management Script

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to start all services
start_all() {
    print_status "Starting Expense Tracker services..."
    docker-compose up --build -d
    print_success "All services started successfully!"
    print_status "Frontend: http://localhost:3000"
    print_status "API: http://localhost:5001"
    print_status "Database: localhost:5432"
}

# Function to stop all services
stop_all() {
    print_status "Stopping Expense Tracker services..."
    docker-compose down
    print_success "All services stopped successfully!"
}

# Function to view logs
view_logs() {
    print_status "Showing logs for all services..."
    docker-compose logs -f
}

# Function to view logs for specific service
view_service_logs() {
    if [ -z "$1" ]; then
        print_error "Please specify a service name (api, frontend, postgres)"
        exit 1
    fi
    print_status "Showing logs for $1 service..."
    docker-compose logs -f $1
}

# Function to check service status
check_status() {
    print_status "Checking service status..."
    docker-compose ps
}

# Function to clean up everything
cleanup() {
    print_warning "This will remove all containers, networks, and volumes!"
    read -p "Are you sure? (y/N): " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        print_status "Cleaning up..."
        docker-compose down -v
        docker system prune -f
        print_success "Cleanup completed!"
    else
        print_status "Cleanup cancelled."
    fi
}

# Function to restart a specific service
restart_service() {
    if [ -z "$1" ]; then
        print_error "Please specify a service name (api, frontend, postgres)"
        exit 1
    fi
    print_status "Restarting $1 service..."
    docker-compose restart $1
    print_success "$1 service restarted!"
}

# Function to show help
show_help() {
    echo "Expense Tracker Docker Management Script"
    echo ""
    echo "Usage: $0 [COMMAND]"
    echo ""
    echo "Commands:"
    echo "  start       Start all services"
    echo "  stop        Stop all services"
    echo "  restart     Restart all services"
    echo "  logs        View logs for all services"
    echo "  logs [service] View logs for specific service"
    echo "  status      Check service status"
    echo "  restart [service] Restart specific service"
    echo "  cleanup     Remove all containers, networks, and volumes"
    echo "  help        Show this help message"
    echo ""
    echo "Services: api, frontend, postgres"
}

# Main script logic
case "$1" in
    start)
        start_all
        ;;
    stop)
        stop_all
        ;;
    restart)
        print_status "Restarting all services..."
        docker-compose restart
        print_success "All services restarted!"
        ;;
    logs)
        if [ -n "$2" ]; then
            view_service_logs $2
        else
            view_logs
        fi
        ;;
    status)
        check_status
        ;;
    restart)
        if [ -n "$2" ]; then
            restart_service $2
        else
            print_error "Please specify a service name"
            exit 1
        fi
        ;;
    cleanup)
        cleanup
        ;;
    help|--help|-h)
        show_help
        ;;
    *)
        print_error "Unknown command: $1"
        show_help
        exit 1
        ;;
esac
