# Maze2D

Abaixo é descrito o contexto da aplicação do algoritmo relacionado à Teoria dos Grafos escolhido para o projeto, assim como as adaptações realizadas de acordo com o funcionamento específico desejado para sua utilização e descrições detalhadas sobre como a teoria do seu funcionamento foi traduzida para a representação visual necessária para o projeto.   

## Contexto

Reforçando a aplicação de grafos no contexto do jogo, o algoritmo utilizado tem como objetivo a geração de labirintos, que serão utilizados como o próprio mapa no qual todas as mecânicas do jogo serão aplicadas. O algoritmo escolhido para o projeto foi a busca em profundidade, uma vez que gera longos caminhos incorretos e oferece a garantia de que qualquer "célula" presente no mapa fará parte de um caminho partindo de outra célula, uma vez que o grafo utilizado para a geração do labirinto é conexo. Este fator é crucial para que o *spawn* do personagem possa ser escolhido estratégicamente, independentemente de qualquer limitação relacionada aos caminhos, visando um maior controle da facilidade/dificuldade de cada nível.

## Especificações

Algumas estratégias em particular foram escolhidas no desenvolvimento do algoritmo para que a funcionalidade da busca em profundidade fosse explorada no contexto do labirinto:

1. Para a geração dos caminhos, uma estrutura de pilha foi utilizada para explicitar o comportamento de *backtracking* necessário para que todas as células fossem exploradas sempre a partir de um caminho raiz, garantindo a alcançabilidade desejada de cada célula.

2. Cada célula possui diretamente quatro vizinhos (acima, abaixo, à esquerda e à direita). Dessa forma, para que fossem gerados labirintos distintos a cada execução, tais vizinhos foram armazenados em um vetor e a escolha de qual seria explorado em seguida após a célula de origem é completamente aleatória.

3. Não existe um parâmetro para a escolha da célula em que a busca terá início, uma célula aleatória do grafo é escolhida como a raiz da árvore de profundidade.

## Implementação (Scripts)

### MazeCell

Responsável por representar o *GameObject* de cada célula que será apresentada no jogo. Possui um vetor de *GameObjects* representando as *sprites* que funcionarão como delimitação para cada vértice, ou seja, as paredes de cada uma das direções. Este atributo está relacionado com a estratégia escolhida para a representação dos caminhos e é explorado da seguinte forma: durante a geração da árvore de profundidade, os caminhos escolhidos no algoritmo contém um conjunto de células que apresentam uma relação de pai e filho. Essa conexão pode ser traduzida omitindo as paredes adjacentes dessas células, gerando a impressão visual de que é possível acessar uma partindo de outra, ao contrário das arestas não presentes na árvore de profundidade, que apresentarão uma parede bloqueando a passagem de uma célula a outra. Cada *GameObject* `MazeCell` possui uma célula correspondente do grafo em que a busca é realizada.

### Cell

Célula (ou vértice) diretamente manipulada pelo algoritmo. É composta principalmente pelos campos `x` e `y` referentes às suas coordenadas, `visited`, para o controle da exploração de vértices na busca em profundidade e uma lista `bool[] flag` com uma posição para cada direção da célula que controla quais delas devem ou não apresentar uma parede. Além disso, o componente apresenta dois métodos: um deles, `SetDirFlag`, altera o estado da lista de direções ativas da célula, propagando a alteração em `MazeCell` para que a parede de fato deixe se tornar visualmente inativa, e `GetNeighbours`, que retorna a lista de vizinhos do componente, utilizada na busca.

### Maze

Representa o conjunto total de células que define o labirinto, consistindo em uma matriz de `Cell` e seu número de linhas e colunas (`mRows` e `mCols`). Seus métodos consistem em `GetCell`, que retorna a celula referente às coordenadas especificadas, `GetUnvisitedNeighbours`, que retorna todos os vizinhos de uma determinada célula que ainda não foram explorados pelo algoritmo de busca e a sua respectiva direção em relação à célula especificada e `RemoveCellWall`, que determina o par de células pai e filho conectadas e altera o estado de ativação de suas delimitações, para que a conexão seja criada.

### MazeGenerator

*Script* principal para a geração do labirinto. Armazena a *prefab* escolhida para o estilo de cada célula, uma matriz de `MazeCell` para a propagação visual de alterações realizadas na matriz utilizada na busca (`Maze maze`) e a pilha para execução da mesma. A partir do método `Start`, toda a geração é engatilhada, de acordo com os seguintes passos:

- Ambas as matrizes são inicializadas com os tamanhos definidos assim como cada célula que as compõem. Para cada célula, um novo *GameObject* é instanciado com o template do *prefab* especificado, e sua posição na interface da *engine* é calculada.

* O procedimento de geração do labirinto é iniciado, primeiramente definindo arbitrariamente a célula de início da busca, com o uso da função `Range` da classe `Random`, e a inserindo na pilha.

- Enquanto a pilha não está vazia, a célula em seu topo é acessada e seus vizinhos ainda não visitados são carregados com uma chamada a `GetUnvisitedNeighbours` da classe `Maze` em uma lista de tuplas compostas por `Directions` e `Cell`, que representam a direção relativa da célula vizinha à celula original (`UP`, `BOTTOM`, `LEFT` e `RIGHT`) e a própria célula vizinha, respectivamente. Logo após, a célula vizinha é escolhida aleatoriamente da mesma maneira que a célula raiz e as paredes conectando-as são removidas (desativadas). Por fim, a nova célula é empilhada e o método é chamado novamente.

* A partir do momento que o acesso aos vizinhos não visitados de uma célula resulta em uma lista vazia, uma das folhas da árvore de profundidade foi atingida, o que resulta no desempilhamento da célula em questão para que possíveis novos caminhos sejam encontrados. 