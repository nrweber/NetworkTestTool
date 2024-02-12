package main

import (
    "fmt"
    "net"
    "time"
)

func main() {
    fmt.Println("starting");
    runTcpListner()
    //runTcpSender()
}



// Tcp Listner
func runTcpListner() {
    listner, err := net.Listen("tcp", ":5000")
    if err != nil {
        fmt.Println(err)
        return
    }

    for {
        connection, err := listner.Accept()
        if err != nil {
            fmt.Println(err)
            continue 
        }

        go handleIncomingConnection(connection)
    }
}


func handleIncomingConnection(connection net.Conn) {
    defer connection.Close()

    for {
        buffer := make([]byte, 1024)
        _, err := connection.Read(buffer)
        if err != nil {
            fmt.Println(err)
            return 
        }

        fmt.Printf("Received: %s\n", buffer) 
    }
}



//TCP Sender
func runTcpSender() {
    for {
        connection, err := net.Dial("tcp", "localhost:5000")
        if err != nil {
            fmt.Println(err)
            time.Sleep(time.Duration(1*time.Second))
            continue
        }

        for {
            _, err = connection.Write([]byte("Hello, server!"))
            if err != nil {
                fmt.Println(err)
                break
            }
            time.Sleep(time.Duration(1*time.Second))
        }
    }
 
    // Didn't need this for this case
    //connection.Close()
}
