package pipeline

import "fmt"

type ProcessingError struct {
	Requeue bool
	Message string
}

func (e *ProcessingError) Error() string {
	return fmt.Sprintf("processing error (requeue=%v): %s", e.Requeue, e.Message)
}
