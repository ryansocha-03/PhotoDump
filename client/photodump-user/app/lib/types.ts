export interface ApiResponseModel<T> {
    code: number,
    data: T | null
}