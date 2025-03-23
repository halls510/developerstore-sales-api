import { UserRole, UserStatus } from 'src/app/models/user.model';
import { User } from '../models/user.model';

export const MOCK_USERS: { data: User[]; success: boolean; message: string; errors: any[] } = {
  data: [
    { 
      id: 1, 
      name: 'Ana Souza', 
      email: 'ana.souza@email.com', 
      role: UserRole.Admin, 
      status: UserStatus.Active, 
      address: {
        street: 'Av. Paulista',
        number: '123',
        city: 'São Paulo',
        state: 'SP',
        zipCode: '01311-000',
        country: 'Brasil',
        geolocation: { lat: -23.561684, long: -46.656139 }
      },
      createdAt: new Date(), 
      updatedAt: new Date() 
    },
    { 
      id: 2, 
      name: 'Carlos Oliveira', 
      email: 'carlos.oliveira@email.com', 
      role: UserRole.Manager, 
      status: UserStatus.Active, 
      address: {
        street: 'Rua XV de Novembro',
        number: '456',
        city: 'Curitiba',
        state: 'PR',
        zipCode: '80020-310',
        country: 'Brasil',
        geolocation: { lat: -25.429594, long: -49.271938 }
      },
      createdAt: new Date(), 
      updatedAt: new Date() 
    },
    { 
      id: 3, 
      name: 'Mariana Lima', 
      email: 'mariana.lima@email.com', 
      role: UserRole.Customer, 
      status: UserStatus.Active, 
      address: {
        street: 'Rua das Flores',
        number: '789',
        city: 'Belo Horizonte',
        state: 'MG',
        zipCode: '30112-010',
        country: 'Brasil',
        geolocation: { lat: -19.922731, long: -43.945095 }
      },
      createdAt: new Date(), 
      updatedAt: new Date() 
    }
  ],
  success: true,
  message: '',
  errors: []
};



export const MOCK_PRODUCTS = { 
	data: [
	  {
		id: 1,
		title: "Cerveja Puro Malte",
		price: 9.99,
		image: "https://minio-api.hallison.com.br/productsapisale/cerveja_puro_malte.png",
		description: "Cerveja puro malte premium, 600ml.",
		category: "Cervejas",
		rating: {
		  rate: 4.5,
		  count: 120
		}
	  },
	  {
		id: 2,
		title: "Cerveja IPA Artesanal",
		price: 14.99,
		image: "https://minio-api.hallison.com.br/productsapisale/cerveja_puro_malte.png",
		description: "Cerveja IPA de alta fermentação, amargor intenso, 500ml.",
		category: "Cervejas",
		rating: {
		  rate: 4.7,
		  count: 95
		}
	  },
	  {
		id: 3,
		title: "Cerveja Lager Premium",
		price: 8.99,
		image: "https://minio-api.hallison.com.br/productsapisale/cerveja_puro_malte.png",
		description: "Cerveja lager leve e refrescante, 600ml.",
		category: "Cervejas",
		rating: {
		  rate: 4.6,
		  count: 130
		}
	  },
	  {
		id: 4,
		title: "Cerveja Weiss Alemã",
		price: 12.99,
		image: "https://minio-api.hallison.com.br/productsapisale/cerveja_puro_malte.png",
		description: "Cerveja de trigo com notas frutadas, 500ml.",
		category: "Cervejas",
		rating: {
		  rate: 4.8,
		  count: 110
		}
	  },
	  {
		id: 5,
		title: "Vinho Tinto Seco",
		price: 59.99,
		image: "https://minio-api.hallison.com.br/productsapisale/cerveja_puro_malte.png",
		description: "Vinho tinto seco de alta qualidade, 750ml.",
		category: "Vinhos",
		rating: {
		  rate: 4.7,
		  count: 150
		}
	  },
	  {
		id: 6,
		title: "Vinho Branco Chardonnay",
		price: 49.99,
		image: "https://minio-api.hallison.com.br/productsapisale/cerveja_puro_malte.png",
		description: "Vinho branco suave e aromático, 750ml.",
		category: "Vinhos",
		rating: {
		  rate: 4.6,
		  count: 100
		}
	  }
	],
	success: true,
  	message: '',
  	errors: []
  };
  

export const MOCK_CATEGORIES = {
  data: ['Cervejas', 'Vinhos'],
  success: true,
  message: '',
  errors: []
};