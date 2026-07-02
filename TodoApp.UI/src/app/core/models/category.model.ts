export interface CategoryDto {
    id: string;
    name: string;
    color?: string | null;
}

export interface CreateCategoryDto {
    name: string;
    color?: string | null;
}