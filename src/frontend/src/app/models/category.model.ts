export interface CategoriesResponse {
    data: string[];   // Lista de categorias
    success: boolean; // Indica se a requisição foi bem-sucedida
    message: string;  // Mensagem informativa
    errors: string[]; // Lista de erros, se houver
  }  